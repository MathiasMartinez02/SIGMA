using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Dashboard.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Dashboard.Queries.GetDashboard;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfPrevMonth = startOfMonth.AddMonths(-1);
        var endOfPrevMonth = startOfMonth.AddTicks(-1);

        // Active Work Orders (current)
        var activeStatuses = new[] { WorkOrderStatus.Pendiente, WorkOrderStatus.EnProceso, WorkOrderStatus.EnInspeccion };
        var activeWorkOrders = await _context.WorkOrders
            .CountAsync(w => !w.IsDeleted && activeStatuses.Contains(w.Status), cancellationToken);

        var prevActiveWorkOrders = await _context.WorkOrders
            .CountAsync(w => !w.IsDeleted && activeStatuses.Contains(w.Status) && w.CreatedAt < startOfMonth, cancellationToken);

        decimal activeWorkOrdersDelta = prevActiveWorkOrders > 0
            ? Math.Round(((decimal)(activeWorkOrders - prevActiveWorkOrders) / prevActiveWorkOrders) * 100, 1)
            : 0;

        // Pending Inspections
        var pendingInspectionStatuses = new[] { InspectionStatus.Pendiente, InspectionStatus.EnProceso };
        var pendingInspections = await _context.Inspections
            .CountAsync(i => !i.IsDeleted && pendingInspectionStatuses.Contains(i.Status), cancellationToken);

        var prevPendingInspections = await _context.Inspections
            .CountAsync(i => !i.IsDeleted && pendingInspectionStatuses.Contains(i.Status) && i.CreatedAt < startOfMonth, cancellationToken);

        decimal pendingInspectionsDelta = prevPendingInspections > 0
            ? Math.Round(((decimal)(pendingInspections - prevPendingInspections) / prevPendingInspections) * 100, 1)
            : 0;

        // Aircraft in Maintenance
        var maintenanceStatuses = new[] { AircraftStatus.EnMantenimiento, AircraftStatus.EnInspeccion };
        var aircraftInMaintenance = await _context.Aircraft
            .CountAsync(a => !a.IsDeleted && maintenanceStatuses.Contains(a.Status), cancellationToken);

        // Completed this month
        var completedThisMonth = await _context.WorkOrders
            .CountAsync(w => !w.IsDeleted && w.Status == WorkOrderStatus.Finalizada
                && w.CompletedDate.HasValue && w.CompletedDate.Value >= startOfMonth, cancellationToken);

        var completedPrevMonth = await _context.WorkOrders
            .CountAsync(w => !w.IsDeleted && w.Status == WorkOrderStatus.Finalizada
                && w.CompletedDate.HasValue
                && w.CompletedDate.Value >= startOfPrevMonth
                && w.CompletedDate.Value <= endOfPrevMonth, cancellationToken);

        decimal completedDelta = completedPrevMonth > 0
            ? Math.Round(((decimal)(completedThisMonth - completedPrevMonth) / completedPrevMonth) * 100, 1)
            : 0;

        // Overdue Items
        var expiredCertificates = await _context.Aircraft
            .CountAsync(a => !a.IsDeleted && a.CertificateExpiry < now, cancellationToken);

        var expiredLicenses = await _context.Users
            .CountAsync(u => !u.IsDeleted && u.IsActive && u.LicenseExpiry.HasValue && u.LicenseExpiry.Value < now, cancellationToken);

        var overdueComponents = await _context.AircraftComponents
            .Include(c => c.Aircraft)
            .CountAsync(c => !c.IsDeleted && !c.Aircraft.IsDeleted
                && c.LifeLimitHours.HasValue
                && c.Aircraft.TotalFlightHours >= (c.InstallHours + c.LifeLimitHours.Value), cancellationToken);

        var overdueItems = expiredCertificates + expiredLicenses + overdueComponents;

        // Work Order Chart - last 7 months
        var chartData = new List<WorkOrderChartDto>();
        for (int i = 6; i >= 0; i--)
        {
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

            var completedCount = await _context.WorkOrders
                .CountAsync(w => !w.IsDeleted && w.Status == WorkOrderStatus.Finalizada
                    && w.CompletedDate.HasValue
                    && w.CompletedDate.Value >= monthStart
                    && w.CompletedDate.Value <= monthEnd, cancellationToken);

            var inProgressCount = await _context.WorkOrders
                .CountAsync(w => !w.IsDeleted && w.Status == WorkOrderStatus.EnProceso
                    && w.CreatedAt >= monthStart && w.CreatedAt <= monthEnd, cancellationToken);

            var pendingCount = await _context.WorkOrders
                .CountAsync(w => !w.IsDeleted && w.Status == WorkOrderStatus.Pendiente
                    && w.CreatedAt >= monthStart && w.CreatedAt <= monthEnd, cancellationToken);

            chartData.Add(new WorkOrderChartDto
            {
                Month = monthStart.ToString("MMM yyyy"),
                Completadas = completedCount,
                EnProceso = inProgressCount,
                Pendientes = pendingCount
            });
        }

        // Inspection Types (approved inspections grouped by type)
        var inspectionTypes = await _context.Inspections
            .Where(i => !i.IsDeleted && i.Status == InspectionStatus.Aprobada)
            .GroupBy(i => i.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var inspectionTypeColors = new[] { "#3b82f6", "#22c55e", "#f59e0b", "#ef4444", "#8b5cf6", "#06b6d4" };
        var inspectionTypeDtos = inspectionTypes.Select((t, idx) => new ChartItemDto
        {
            Name = t.Type,
            Value = t.Count,
            Color = inspectionTypeColors[idx % inspectionTypeColors.Length]
        }).ToList();

        // Aircraft by Status
        var aircraftStatusColors = new Dictionary<string, string>
        {
            { "Operativa", "#22c55e" },
            { "EnMantenimiento", "#f59e0b" },
            { "FueraDeServicio", "#ef4444" },
            { "EnInspeccion", "#3b82f6" }
        };

        var aircraftByStatusRaw = await _context.Aircraft
            .Where(a => !a.IsDeleted)
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var aircraftByStatus = aircraftByStatusRaw.Select(s => new ChartItemDto
        {
            Name = s.Status.ToString(),
            Value = s.Count,
            Color = aircraftStatusColors.TryGetValue(s.Status.ToString(), out var color) ? color : "#6b7280"
        }).ToList();

        // Upcoming Expiries (next 60 days)
        var in60Days = now.AddDays(60);
        var upcomingExpiries = new List<UpcomingExpiryDto>();

        // Aircraft certificates
        var expiringCerts = await _context.Aircraft
            .Include(a => a.Client)
            .Where(a => !a.IsDeleted && a.CertificateExpiry >= now && a.CertificateExpiry <= in60Days)
            .ToListAsync(cancellationToken);

        foreach (var aircraft in expiringCerts)
        {
            var daysUntil = (int)(aircraft.CertificateExpiry - now).TotalDays;
            upcomingExpiries.Add(new UpcomingExpiryDto
            {
                Id = aircraft.Id,
                Type = "certificado",
                Description = $"Certificado de aeronavegabilidad - {aircraft.Registration}",
                Aircraft = aircraft.Registration,
                DueDate = aircraft.CertificateExpiry,
                DaysUntilDue = daysUntil,
                Priority = daysUntil <= 7 ? "critical" : daysUntil <= 30 ? "warning" : "normal"
            });
        }

        // User licenses
        var expiringLicenses = await _context.Users
            .Where(u => !u.IsDeleted && u.IsActive && u.LicenseExpiry.HasValue
                && u.LicenseExpiry.Value >= now && u.LicenseExpiry.Value <= in60Days)
            .ToListAsync(cancellationToken);

        foreach (var user in expiringLicenses)
        {
            var daysUntil = (int)(user.LicenseExpiry!.Value - now).TotalDays;
            upcomingExpiries.Add(new UpcomingExpiryDto
            {
                Id = user.Id,
                Type = "licencia",
                Description = $"Licencia - {user.FirstName} {user.LastName}",
                Aircraft = null,
                DueDate = user.LicenseExpiry.Value,
                DaysUntilDue = daysUntil,
                Priority = daysUntil <= 7 ? "critical" : daysUntil <= 30 ? "warning" : "normal"
            });
        }

        // Upcoming inspections (scheduled in next 60 days)
        var upcomingInspections = await _context.Inspections
            .Include(i => i.Aircraft)
            .Where(i => !i.IsDeleted
                && i.Status == InspectionStatus.Pendiente
                && i.ScheduledDate >= now && i.ScheduledDate <= in60Days)
            .ToListAsync(cancellationToken);

        foreach (var inspection in upcomingInspections)
        {
            var daysUntil = (int)(inspection.ScheduledDate - now).TotalDays;
            upcomingExpiries.Add(new UpcomingExpiryDto
            {
                Id = inspection.Id,
                Type = "inspeccion",
                Description = $"Inspección {inspection.Type} - {inspection.Aircraft.Registration}",
                Aircraft = inspection.Aircraft.Registration,
                DueDate = inspection.ScheduledDate,
                DaysUntilDue = daysUntil,
                Priority = daysUntil <= 7 ? "critical" : daysUntil <= 30 ? "warning" : "normal"
            });
        }

        upcomingExpiries = upcomingExpiries.OrderBy(e => e.DaysUntilDue).ToList();

        // Recent Activity - last 20 WorkOrderTimeline entries
        var recentTimelines = await _context.WorkOrderTimelines
            .Include(t => t.WorkOrder)
            .OrderByDescending(t => t.CreatedAt)
            .Take(20)
            .ToListAsync(cancellationToken);

        var recentActivity = recentTimelines.Select(t => new RecentActivityDto
        {
            Id = t.Id,
            Type = t.Event,
            Description = t.Description,
            UserId = t.UserId,
            UserName = t.UserName,
            UserRole = t.UserRole,
            ReferenceId = t.WorkOrderId,
            ReferenceNumber = t.WorkOrder.Number,
            CreatedAt = t.CreatedAt
        }).ToList();

        return new DashboardDto
        {
            Kpis = new KpisDto
            {
                ActiveWorkOrders = activeWorkOrders,
                ActiveWorkOrdersDelta = activeWorkOrdersDelta,
                PendingInspections = pendingInspections,
                PendingInspectionsDelta = pendingInspectionsDelta,
                AircraftInMaintenance = aircraftInMaintenance,
                CompletedThisMonth = completedThisMonth,
                CompletedThisMonthDelta = completedDelta,
                OverdueItems = overdueItems
            },
            WorkOrderChart = chartData,
            InspectionTypes = inspectionTypeDtos,
            AircraftByStatus = aircraftByStatus,
            UpcomingExpiries = upcomingExpiries,
            RecentActivity = recentActivity
        };
    }
}
