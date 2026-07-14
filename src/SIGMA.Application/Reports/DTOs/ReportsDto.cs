using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Inspections.DTOs;
using SIGMA.Application.Inventory.DTOs;
using SIGMA.Application.WorkOrders.DTOs;

namespace SIGMA.Application.Reports.DTOs;

public class WorkOrdersReportDto
{
    public IList<WorkOrderReportItemDto> WorkOrders { get; init; } = [];
    public int TotalCount { get; init; }
    public decimal AverageHours { get; init; }
    public Dictionary<string, int> ByStatus { get; init; } = [];
    public Dictionary<string, int> ByType { get; init; } = [];
}

public class WorkOrderReportItemDto
{
    public Guid Id { get; init; }
    public string Number { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public string AircraftRegistration { get; init; } = string.Empty;
    public string AircraftModel { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal EstimatedHours { get; init; }
    public decimal ActualHours { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime EstimatedEndDate { get; init; }
    public DateTime? CompletedDate { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class InspectionsReportDto
{
    public IList<InspectionDto> Inspections { get; init; } = [];
    public int TotalCount { get; init; }
    public Dictionary<string, int> ByStatus { get; init; } = [];
    public Dictionary<string, int> ByType { get; init; } = [];
    // Cantidad de inspecciones completadas (Completada/Aprobada/Rechazada, es decir con CompletedAt cargado) dentro del periodo
    public int CompletedCount { get; init; }
    // De las completadas, cuantas se realizaron en o antes de la fecha programada (ScheduledDate)
    public int OnTimeCount { get; init; }
    // Porcentaje de cumplimiento normativo (OnTimeCount / CompletedCount). Null si no hay inspecciones completadas en el periodo
    public decimal? CompliancePercentage { get; init; }
}

// Item de horas de taller (ActualHours de OT) agrupadas por mes, usado para el grafico "Horas de Taller por Mes"
public class MonthlyWorkshopHoursItemDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public string Label { get; init; } = string.Empty;
    public decimal TotalHours { get; init; }
}

public class MonthlyWorkshopHoursReportDto
{
    public IList<MonthlyWorkshopHoursItemDto> Items { get; init; } = [];
}

public class AircraftStatusReportDto
{
    public IList<AircraftDto> Aircraft { get; init; } = [];
    public Dictionary<string, int> ByStatus { get; init; } = [];
    public Dictionary<string, int> ByCategory { get; init; } = [];
}

public class InventoryReportDto
{
    public IList<InventoryItemDto> Items { get; init; } = [];
    public int LowStockCount { get; init; }
    public int OutOfStockCount { get; init; }
    public int ExpiredCount { get; init; }
    public decimal TotalValue { get; init; }
}
