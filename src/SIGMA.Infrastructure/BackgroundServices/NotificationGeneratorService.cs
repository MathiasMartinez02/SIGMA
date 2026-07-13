using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Domain.Entities;
using SIGMA.Domain.Enums;

namespace SIGMA.Infrastructure.BackgroundServices;

// Servicio en background que cada 30 minutos escanea vencimientos de aeronaves, documentos y stock
// y genera notificaciones persistidas para todos los usuarios activos, evitando duplicados no leidos.
// Simplificacion de MVP: notifica a todos los usuarios activos (User.IsActive), sin segmentar por rol/area.
public class NotificationGeneratorService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan ExpiryWindow = TimeSpan.FromDays(30);

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

        var created = 0;
        created += await NotifyAircraftInspectionsAsync(context, activeUserIds, now, threshold, cancellationToken);
        created += await NotifyAircraftCertificatesAsync(context, activeUserIds, now, threshold, cancellationToken);
        created += await NotifyAircraftDocumentsAsync(context, activeUserIds, now, threshold, cancellationToken);
        created += await NotifyInventoryAsync(context, activeUserIds, cancellationToken);

        if (created > 0)
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
            var alreadyExists = await ExistsUnreadNotificationAsync(context, NotificationType.InspeccionProxima, aircraft.Id, cancellationToken);
            if (alreadyExists) continue;

            var title = "Inspeccion proxima";
            var message = $"La aeronave {aircraft.Registration} tiene una inspeccion programada para el {aircraft.NextInspectionDue:dd/MM/yyyy}.";
            count += CreateForActiveUsers(context, activeUserIds, NotificationType.InspeccionProxima, title, message, aircraft.Id);
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
            var alreadyExists = await ExistsUnreadNotificationAsync(context, NotificationType.CertificadoPorVencer, aircraft.Id, cancellationToken);
            if (alreadyExists) continue;

            var title = "Certificado por vencer";
            var message = $"El certificado de aeronavegabilidad de {aircraft.Registration} vence el {aircraft.CertificateExpiry:dd/MM/yyyy}.";
            count += CreateForActiveUsers(context, activeUserIds, NotificationType.CertificadoPorVencer, title, message, aircraft.Id);
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
            var alreadyExists = await ExistsUnreadNotificationAsync(context, NotificationType.DocumentoPorVencer, document.Id, cancellationToken);
            if (alreadyExists) continue;

            var title = "Documento por vencer";
            var message = $"El documento '{document.Name}' de la aeronave {document.Aircraft.Registration} vence el {document.ExpiryDate:dd/MM/yyyy}.";
            count += CreateForActiveUsers(context, activeUserIds, NotificationType.DocumentoPorVencer, title, message, document.Id);
        }

        return count;
    }

    // Genera notificaciones de stock (StockBajo, StockSobre, StockVencido) segun el Status actual del InventoryItem
    private async Task<int> NotifyInventoryAsync(IApplicationDbContext context, List<Guid> activeUserIds, CancellationToken cancellationToken)
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

            var alreadyExists = await ExistsUnreadNotificationAsync(context, type, item.Id, cancellationToken);
            if (alreadyExists) continue;

            var (title, message) = type switch
            {
                NotificationType.StockSobre => ("Stock por encima del maximo",
                    $"El item {item.PartNumber} supera el stock maximo configurado (actual: {item.CurrentStock}, maximo: {item.MaximumStock})."),
                NotificationType.StockVencido => ("Item de stock vencido",
                    $"El item {item.PartNumber} tiene fecha de vencimiento {item.ExpiryDate:dd/MM/yyyy}."),
                _ => ("Stock bajo",
                    $"El item {item.PartNumber} tiene stock bajo o sin stock (actual: {item.CurrentStock}, minimo: {item.MinimumStock}).")
            };

            count += CreateForActiveUsers(context, activeUserIds, type, title, message, item.Id);
        }

        return count;
    }

    // Verifica si ya existe una notificacion no leida con el mismo tipo y entidad relacionada, para evitar duplicados
    private static async Task<bool> ExistsUnreadNotificationAsync(
        IApplicationDbContext context, NotificationType type, Guid relatedEntityId, CancellationToken cancellationToken) =>
        await context.Notifications.AnyAsync(
            n => !n.IsRead && n.Type == type && n.RelatedEntityId == relatedEntityId, cancellationToken);

    // Crea una notificacion identica para cada usuario activo y la agrega al contexto sin guardar todavia
    private static int CreateForActiveUsers(
        IApplicationDbContext context, List<Guid> activeUserIds, NotificationType type, string title, string message, Guid relatedEntityId)
    {
        foreach (var userId in activeUserIds)
        {
            var notification = Notification.Create(userId, type, title, message, relatedEntityId);
            context.Notifications.Add(notification);
        }

        return activeUserIds.Count;
    }
}
