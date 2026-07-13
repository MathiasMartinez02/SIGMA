using SIGMA.Domain.Enums;

namespace SIGMA.Application.WorkOrders.DTOs;

public class WorkOrderDto
{
    public Guid Id { get; init; }
    public string Number { get; init; } = string.Empty;
    public WorkOrderType Type { get; init; }
    public WorkOrderStatus Status { get; init; }
    public WorkOrderPriority Priority { get; init; }
    public Guid AircraftId { get; init; }
    public string AircraftRegistration { get; init; } = string.Empty;
    public string AircraftModel { get; init; } = string.Empty;
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal EstimatedHours { get; init; }
    public decimal ActualHours { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime IntakeDate { get; init; }
    public DateTime EstimatedEndDate { get; init; }
    public DateTime? CompletedDate { get; init; }
    public decimal AircraftHoursAtStart { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int TaskCount { get; init; }
    public int CompletedTaskCount { get; init; }
}

public class WorkOrderDetailDto : WorkOrderDto
{
    public IList<WorkOrderTaskDto> Tasks { get; init; } = [];
    public IList<WorkOrderTimelineDto> Timeline { get; init; } = [];
    public IList<WorkOrderMaterialDto> Materials { get; init; } = [];
    public IList<WorkOrderDocumentDto> Documents { get; init; } = [];
    public IList<AssignedMechanicDto> AssignedMechanics { get; init; } = [];
}

public class WorkOrderTaskDto
{
    public Guid Id { get; init; }
    public int OrderIndex { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public WorkOrderTaskStatus Status { get; init; }
    public Guid? AssignedToId { get; init; }
    public string? AssignedToName { get; init; }
    public decimal EstimatedHours { get; init; }
    public decimal ActualHours { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? Observations { get; init; }
    public bool RequiresInspection { get; init; }
    public Guid? InspectedById { get; init; }
    public string? InspectedByName { get; init; }
    public DateTime? InspectedAt { get; init; }
}

public class WorkOrderTimelineDto
{
    public Guid Id { get; init; }
    public string Event { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string UserRole { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? Metadata { get; init; }
}

public class WorkOrderMaterialDto
{
    public Guid Id { get; init; }
    public string PartNumber { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public DateTime RequestedAt { get; init; }
    public DateTime? DeliveredAt { get; init; }
    public string Status { get; init; } = string.Empty;
}

public class WorkOrderDocumentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public Guid UploadedById { get; init; }
    public DateTime UploadedAt { get; init; }
}

public class AssignedMechanicDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime AssignedAt { get; init; }
}
