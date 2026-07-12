using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Domain.Entities;

public class WorkOrderTask : AuditableEntity
{
    public Guid WorkOrderId { get; private set; }
    public WorkOrder WorkOrder { get; private set; } = null!;
    public int OrderIndex { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public WorkOrderTaskStatus Status { get; private set; } = WorkOrderTaskStatus.Pendiente;
    public Guid? AssignedToId { get; private set; }
    public User? AssignedTo { get; private set; }
    public decimal EstimatedHours { get; private set; }
    public decimal ActualHours { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Observations { get; private set; }
    public bool RequiresInspection { get; private set; }
    public Guid? InspectedById { get; private set; }
    public User? InspectedBy { get; private set; }
    public DateTime? InspectedAt { get; private set; }

    private WorkOrderTask() { }

    public static WorkOrderTask Create(
        Guid workOrderId, int orderIndex, string title,
        string description, decimal estimatedHours, bool requiresInspection) =>
        new()
        {
            WorkOrderId = workOrderId,
            OrderIndex = orderIndex,
            Title = title.Trim(),
            Description = description.Trim(),
            EstimatedHours = estimatedHours,
            RequiresInspection = requiresInspection
        };

    public void UpdateStatus(WorkOrderTaskStatus newStatus, string? observations, Guid? userId = null)
    {
        Status = newStatus;
        Observations = observations;

        if (newStatus == WorkOrderTaskStatus.Completada)
            CompletedAt = DateTime.UtcNow;

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkInspected(Guid inspectorId)
    {
        if (!RequiresInspection)
            throw new DomainException("Esta tarea no requiere inspección.");
        InspectedById = inspectorId;
        InspectedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Assign(Guid userId)
    {
        AssignedToId = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}
