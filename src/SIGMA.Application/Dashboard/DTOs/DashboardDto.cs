namespace SIGMA.Application.Dashboard.DTOs;

public class DashboardDto
{
    public KpisDto Kpis { get; init; } = null!;
    public IList<WorkOrderChartDto> WorkOrderChart { get; init; } = [];
    public IList<ChartItemDto> InspectionTypes { get; init; } = [];
    public IList<ChartItemDto> AircraftByStatus { get; init; } = [];
    public IList<UpcomingExpiryDto> UpcomingExpiries { get; init; } = [];
    public IList<RecentActivityDto> RecentActivity { get; init; } = [];
}

public class KpisDto
{
    public int ActiveWorkOrders { get; init; }
    public decimal ActiveWorkOrdersDelta { get; init; }
    public int PendingInspections { get; init; }
    public decimal PendingInspectionsDelta { get; init; }
    public int AircraftInMaintenance { get; init; }
    public int CompletedThisMonth { get; init; }
    public decimal CompletedThisMonthDelta { get; init; }
    public int OverdueItems { get; init; }
}

public class WorkOrderChartDto
{
    public string Month { get; init; } = string.Empty;
    public int Completadas { get; init; }
    public int EnProceso { get; init; }
    public int Pendientes { get; init; }
}

public class ChartItemDto
{
    public string Name { get; init; } = string.Empty;
    public int Value { get; init; }
    public string Color { get; init; } = string.Empty;
}

public class UpcomingExpiryDto
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Aircraft { get; init; }
    public DateTime DueDate { get; init; }
    public int DaysUntilDue { get; init; }
    public string Priority { get; init; } = "normal";
}

public class RecentActivityDto
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string UserRole { get; init; } = string.Empty;
    public Guid? ReferenceId { get; init; }
    public string? ReferenceNumber { get; init; }
    public DateTime CreatedAt { get; init; }
}
