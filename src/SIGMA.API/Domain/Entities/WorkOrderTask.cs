using SIGMA.API.Common;
using SIGMA.API.Enums;
using SIGMA.API.Exceptions;

namespace SIGMA.API.Entities;

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

    private static readonly Dictionary<WorkOrderTaskStatus, WorkOrderTaskStatus[]> ValidTransitions = new()
    {
        [WorkOrderTaskStatus.Pendiente] = [WorkOrderTaskStatus.EnProceso],
        [WorkOrderTaskStatus.EnProceso] = [WorkOrderTaskStatus.Completada, WorkOrderTaskStatus.Observada, WorkOrderTaskStatus.Pendiente],
        [WorkOrderTaskStatus.Observada] = [WorkOrderTaskStatus.EnProceso],
        [WorkOrderTaskStatus.Completada] = [WorkOrderTaskStatus.EnProceso],
    };

    // Cambio de estado validado; completar exige horas reales y reabrir limpia el cierre
    public void UpdateStatus(WorkOrderTaskStatus newStatus, string? observations, decimal? actualHours = null)
    {
        if (newStatus != Status && !ValidTransitions[Status].Contains(newStatus))
            throw new DomainException($"La tarea no puede pasar de {Status} a {newStatus}.");

        if (newStatus == WorkOrderTaskStatus.Completada)
        {
            if (actualHours is null or <= 0)
                throw new DomainException("Indicá las horas reales trabajadas para completar la tarea.");
            ActualHours = actualHours.Value;
            CompletedAt ??= DateTime.UtcNow;
        }
        else if (Status == WorkOrderTaskStatus.Completada)
        {
            if (InspectedAt is not null)
                throw new DomainException("La tarea ya fue inspeccionada y no se puede reabrir.");
            CompletedAt = null;
        }

        Status = newStatus;
        Observations = observations;
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

    // Asigna o quita (null) el responsable de la tarea
    public void Assign(Guid? userId)
    {
        AssignedToId = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}
