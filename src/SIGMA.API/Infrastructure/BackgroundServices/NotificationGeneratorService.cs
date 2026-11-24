using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Entities;
using SIGMA.API.Enums;

namespace SIGMA.API.BackgroundServices;

public class NotificationGeneratorService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan ExpiryWindow = TimeSpan.FromDays(30);
    private static readonly TimeSpan RenotifyAfter = TimeSpan.FromDays(7);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationGeneratorService> _logger;

    public NotificationGeneratorService(IServiceScopeFactory scopeFactory, ILogger<NotificationGeneratorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    // Bucle principal del hosted service: ejecuta una corrida inmediata y luego cada 30 minutos hasta que se cancele
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);

        do
        {
            try
            {
                await GenerateNotificationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando notificaciones de vencimientos.");
            }
        }
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
    }

    // Ejecuta una corrida completa de generacion de notificaciones dentro de un scope nuevo (DbContext es scoped)
    private async Task GenerateNotificationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var now = DateTime.UtcNow;
        var threshold = now.Add(ExpiryWindow);

        var activeUserIds = await context.Users
            .Where(u => !u.IsDeleted && u.IsActive)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        if (activeUserIds.Count == 0)
        {
            _logger.LogInformation("No hay usuarios activos, se omite la generacion de notificaciones.");
            return;
        }

        await ExpireInventoryItemsAsync(context, cancellationToken);

        var created = 0;
        created += await NotifyAircraftInspectionsAsync(context, activeUserIds, now, threshold, cancellationToken);
        created += await NotifyAircraftCertificatesAsync(context, activeUserIds, now, threshold, cancellationToken);
        created += await NotifyAircraftDocumentsAsync(context, activeUserIds, now, threshold, cancellationToken);
        created += await NotifyTechnicalDocumentsAsync(context, activeUserIds, now, threshold, cancellationToken);
        created += await NotifyInventoryAsync(context, activeUserIds, now, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Generacion de notificaciones finalizada. Notificaciones creadas: {Created}.", created);
    }

    // Genera notificaciones de tipo InspeccionProxima para aeronaves con NextInspectionDue dentro de la ventana
    private async Task<int> NotifyAircraftInspectionsAsync(
        IApplicationDbContext context, List<Guid> activeUserIds, DateTime now, DateTime threshold, CancellationToken cancellationToken)
    {
        var aircraftDue = await context.Aircraft
            .Where(a => !a.IsDeleted && a.NextInspectionDue.HasValue
                && a.NextInspectionDue.Value >= now && a.NextInspectionDue.Value <= threshold)
            .ToListAsync(cancellationToken);

        var count = 0;
        foreach (var aircraft in aircraftDue)
        {
            var recipients = await PendingRecipientsAsync(context, activeUserIds, NotificationType.InspeccionProxima, aircraft.Id, now, cancellationToken);
            if (recipients.Count == 0) continue;

            var title = "Inspeccion proxima";
            var message = $"La aeronave {aircraft.Registration} tiene una inspeccion programada para el {aircraft.NextInspectionDue:dd/MM/yyyy}.";
            count += CreateForUsers(context, recipients, NotificationType.InspeccionProxima, title, message, aircraft.Id);
        }

        return count;
    }

    // Genera notificaciones de tipo CertificadoPorVencer para aeronaves con CertificateExpiry dentro de la ventana
    private async Task<int> NotifyAircraftCertificatesAsync(
        IApplicationDbContext context, List<Guid> activeUserIds, DateTime now, DateTime threshold, CancellationToken cancellationToken)
    {
        var aircraftExpiring = await context.Aircraft
            .Where(a => !a.IsDeleted && a.CertificateExpiry >= now && a.CertificateExpiry <= threshold)
            .ToListAsync(cancellationToken);

        var count = 0;
        foreach (var aircraft in aircraftExpiring)
        {
            var recipients = await PendingRecipientsAsync(context, activeUserIds, NotificationType.CertificadoPorVencer, aircraft.Id, now, cancellationToken);
            if (recipients.Count == 0) continue;

            var title = "Certificado por vencer";
            var message = $"El certificado de aeronavegabilidad de {aircraft.Registration} vence el {aircraft.CertificateExpiry:dd/MM/yyyy}.";
            count += CreateForUsers(context, recipients, NotificationType.CertificadoPorVencer, title, message, aircraft.Id);
        }

        return count;
    }

    // Genera notificaciones de tipo DocumentoPorVencer para documentos de aeronave con ExpiryDate dentro de la ventana
    private async Task<int> NotifyAircraftDocumentsAsync(
        IApplicationDbContext context, List<Guid> activeUserIds, DateTime now, DateTime threshold, CancellationToken cancellationToken)
    {
        var documentsExpiring = await context.AircraftDocuments
            .Include(d => d.Aircraft)
            .Where(d => !d.IsDeleted && d.ExpiryDate.HasValue
                && d.ExpiryDate.Value >= now && d.ExpiryDate.Value <= threshold)
            .ToListAsync(cancellationToken);

        var count = 0;
        foreach (var document in documentsExpiring)
        {
            var recipients = await PendingRecipientsAsync(context, activeUserIds, NotificationType.DocumentoPorVencer, document.Id, now, cancellationToken);
            if (recipients.Count == 0) continue;

            var title = "Documento por vencer";
            var message = $"El documento '{document.Name}' de la aeronave {document.Aircraft.Registration} vence el {document.ExpiryDate:dd/MM/yyyy}.";
            count += CreateForUsers(context, recipients, NotificationType.DocumentoPorVencer, title, message, document.Id);
        }

        return count;
    }

    // Avisos de documentos tecnicos por vencer
    private async Task<int> NotifyTechnicalDocumentsAsync(
        IApplicationDbContext context, List<Guid> activeUserIds, DateTime now, DateTime threshold, CancellationToken cancellationToken)
    {
        var documentsExpiring = await context.TechnicalDocuments
            .Where(d => !d.IsDeleted && d.ExpiryDate.HasValue
                && d.ExpiryDate.Value >= now && d.ExpiryDate.Value <= threshold)
            .ToListAsync(cancellationToken);

        var count = 0;
        foreach (var document in documentsExpiring)
        {
            var recipients = await PendingRecipientsAsync(context, activeUserIds, NotificationType.DocumentoPorVencer, document.Id, now, cancellationToken);
            if (recipients.Count == 0) continue;

            var title = "Documento por vencer";
            var message = $"El documento tecnico '{document.Title}' ({document.Type}) vence el {document.ExpiryDate:dd/MM/yyyy}.";
            count += CreateForUsers(context, recipients, NotificationType.DocumentoPorVencer, title, message, document.Id);
        }

        return count;
    }

    // Genera notificaciones de stock (StockBajo, StockSobre, StockVencido) segun el Status actual del InventoryItem
    private async Task<int> NotifyInventoryAsync(IApplicationDbContext context, List<Guid> activeUserIds, DateTime now, CancellationToken cancellationToken)
    {
        var items = await context.InventoryItems
            .Where(i => !i.IsDeleted && (
                i.Status == InventoryStatus.BajoStock ||
                i.Status == InventoryStatus.SinStock ||
                i.Status == InventoryStatus.SobreStock ||
                i.Status == InventoryStatus.Vencido))
            .ToListAsync(cancellationToken);

        var count = 0;
        foreach (var item in items)
        {
            // SinStock se mapea a StockBajo para no agregar un tipo de notificacion adicional
            var type = item.Status switch
            {
                InventoryStatus.SobreStock => NotificationType.StockSobre,
                InventoryStatus.Vencido => NotificationType.StockVencido,
                _ => NotificationType.StockBajo
            };

            var recipients = await PendingRecipientsAsync(context, activeUserIds, type, item.Id, now, cancellationToken);
            if (recipients.Count == 0) continue;

            var (title, message) = type switch
            {
                NotificationType.StockSobre => ("Stock por encima del maximo",
                    $"El item {item.PartNumber} supera el stock maximo configurado (actual: {item.CurrentStock}, maximo: {item.MaximumStock})."),
                NotificationType.StockVencido => ("Item de stock vencido",
                    $"El item {item.PartNumber} tiene fecha de vencimiento {item.ExpiryDate:dd/MM/yyyy}."),
                _ => ("Stock bajo",
                    $"El item {item.PartNumber} tiene stock bajo o sin stock (actual: {item.CurrentStock}, minimo: {item.MinimumStock}).")
            };

            count += CreateForUsers(context, recipients, type, title, message, item.Id);
        }

        return count;
    }

    // Marca como vencidos los items cuya fecha de vencimiento ya paso
    private static async Task ExpireInventoryItemsAsync(IApplicationDbContext context, CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        var expired = await context.InventoryItems
            .Where(i => !i.IsDeleted && i.Status != InventoryStatus.Vencido
                && i.ExpiryDate.HasValue && i.ExpiryDate.Value < today)
            .ToListAsync(cancellationToken);

        foreach (var item in expired)
            item.RecalculateStatus();
    }

    // Usuarios activos sin aviso pendiente ni reciente para esa entidad
    private static async Task<List<Guid>> PendingRecipientsAsync(
        IApplicationDbContext context, List<Guid> activeUserIds, NotificationType type, Guid relatedEntityId,
        DateTime now, CancellationToken cancellationToken)
    {
        var since = now - RenotifyAfter;
        var alreadyNotified = await context.Notifications
            .Where(n => n.Type == type && n.RelatedEntityId == relatedEntityId && (!n.IsRead || n.CreatedAt >= since))
            .Select(n => n.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return activeUserIds.Except(alreadyNotified).ToList();
    }

    private static int CreateForUsers(
        IApplicationDbContext context, List<Guid> userIds, NotificationType type, string title, string message, Guid relatedEntityId)
    {
        foreach (var userId in userIds)
            context.Notifications.Add(Notification.Create(userId, type, title, message, relatedEntityId));

        return userIds.Count;
    }
}
