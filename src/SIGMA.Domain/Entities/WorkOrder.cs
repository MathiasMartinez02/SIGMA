using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Events;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Domain.Entities;

public class WorkOrder : AuditableEntity
{
    public string Number { get; private set; } = string.Empty;
    public WorkOrderType Type { get; private set; }
    public WorkOrderStatus Status { get; private set; } = WorkOrderStatus.Pendiente;
    public WorkOrderPriority Priority { get; private set; }
    public Guid AircraftId { get; private set; }
    public Aircraft Aircraft { get; private set; } = null!;
    public Guid ClientId { get; private set; }
    public Client Client { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public decimal EstimatedHours { get; private set; }
    public decimal ActualHours { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime EstimatedEndDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public decimal AircraftHoursAtStart { get; private set; }

    public ICollection<WorkOrderTask> Tasks { get; private set; } = [];
    public ICollection<WorkOrderTimeline> Timeline { get; private set; } = [];
    public ICollection<WorkOrderMaterial> Materials { get; private set; } = [];
    public ICollection<WorkOrderDocument> Documents { get; private set; } = [];
    public ICollection<AssignedMechanic> AssignedMechanics { get; private set; } = [];
    public ICollection<Inspection> Inspections { get; private set; } = [];

    private static readonly Dictionary<WorkOrderStatus, IReadOnlyList<WorkOrderStatus>> ValidTransitions = new()
    {
        [WorkOrderStatus.Pendiente] = [WorkOrderStatus.EnProceso, WorkOrderStatus.Cancelada],
        [WorkOrderStatus.EnProceso] = [WorkOrderStatus.EnInspeccion, WorkOrderStatus.Pendiente, WorkOrderStatus.Cancelada],
        [WorkOrderStatus.EnInspeccion] = [WorkOrderStatus.Finalizada, WorkOrderStatus.EnProceso],
        [WorkOrderStatus.Finalizada] = [],
        [WorkOrderStatus.Cancelada] = []
    };

    private WorkOrder() { }

    public static WorkOrder Create(
        string number, WorkOrderType type, WorkOrderPriority priority,
        Guid aircraftId, Guid clientId, string description,
        decimal estimatedHours, DateTime estimatedEndDate,
        decimal aircraftHoursAtStart, Guid createdById)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La descripción de la orden de trabajo es requerida.");
        if (estimatedHours <= 0)
            throw new DomainException("Las horas estimadas deben ser mayores a cero.");

        var workOrder = new WorkOrder
        {
            Number = number,
            Type = type,
            Priority = priority,
            AircraftId = aircraftId,
            ClientId = clientId,
            Description = description.Trim(),
            EstimatedHours = estimatedHours,
            EstimatedEndDate = estimatedEndDate,
            AircraftHoursAtStart = aircraftHoursAtStart,
            CreatedById = createdById
        };

        workOrder.AddDomainEvent(new WorkOrderCreatedEvent(workOrder.Id, number, createdById));
        return workOrder;
    }

    public void TransitionTo(WorkOrderStatus newStatus, Guid userId, string userName, string userRole)
    {
        if (!ValidTransitions[Status].Contains(newStatus))
            throw new InvalidStatusTransitionException(Status.ToString(), newStatus.ToString());

        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        if (newStatus == WorkOrderStatus.EnProceso && !StartDate.HasValue)
            StartDate = DateTime.UtcNow;

        if (newStatus == WorkOrderStatus.Finalizada)
        {
            EnsureAllInspectionsCompleted();
            CompletedDate = DateTime.UtcNow;
        }

        var timelineEntry = WorkOrderTimeline.Create(
            Id, $"Estado: {oldStatus} → {newStatus}",
            $"Estado cambiado de '{oldStatus}' a '{newStatus}'",
            userId, userName, userRole);

        Timeline.Add(timelineEntry);
    }

    private void EnsureAllInspectionsCompleted()
    {
        var pendingInspectionTasks = Tasks
            .Where(t => t.RequiresInspection && t.Status != WorkOrderTaskStatus.Completada)
            .ToList();

        if (pendingInspectionTasks.Count != 0)
            throw new DomainException(
                $"No se puede finalizar la OT. Hay {pendingInspectionTasks.Count} tarea(s) con inspección pendiente.");
    }

    public void Update(string description, decimal estimatedHours, DateTime estimatedEndDate, WorkOrderPriority priority)
    {
        Description = description.Trim();
        EstimatedHours = estimatedHours;
        EstimatedEndDate = estimatedEndDate;
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateActualHours(decimal hours)
    {
        ActualHours = hours;
        UpdatedAt = DateTime.UtcNow;
    }
}
