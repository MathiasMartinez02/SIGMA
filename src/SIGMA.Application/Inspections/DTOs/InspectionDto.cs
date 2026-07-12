using SIGMA.Domain.Enums;

namespace SIGMA.Application.Inspections.DTOs;

public class InspectionDto
{
    public Guid Id { get; init; }
    public Guid WorkOrderId { get; init; }
    public string WorkOrderNumber { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public InspectionStatus Status { get; init; }
    public Guid AircraftId { get; init; }
    public string AircraftRegistration { get; init; } = string.Empty;
    public decimal AircraftHours { get; init; }
    public Guid? InspectorId { get; init; }
    public string? InspectorName { get; init; }
    public DateTime ScheduledDate { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public string? OverallResult { get; init; }
    public string? Observations { get; init; }
    public string? RejectionReason { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class InspectionDetailDto : InspectionDto
{
    public IList<ChecklistSectionDto> ChecklistSections { get; init; } = [];
}

public class ChecklistSectionDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int OrderIndex { get; init; }
    public IList<ChecklistItemDto> Items { get; init; } = [];
}

public class ChecklistItemDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Reference { get; init; }
    public int OrderIndex { get; init; }
    public ChecklistItemStatus Status { get; init; }
    public string? Observations { get; init; }
    public Guid? CheckedById { get; init; }
    public DateTime? CheckedAt { get; init; }
    public bool RequiresPhoto { get; init; }
    public string? PhotoUrl { get; init; }
}
