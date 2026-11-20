using SIGMA.API.Common;
using SIGMA.API.Enums;
using SIGMA.API.Events;
using SIGMA.API.Exceptions;

namespace SIGMA.API.Entities;

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
    public DateTime IntakeDate { get; private set; }
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

    // MODIFICADO: agrega el parametro intakeDate (fecha de ingreso de la aeronave al taller, dato de negocio distinto de CreatedAt)
    // ANTERIOR: Create(string number, WorkOrderType type, WorkOrderPriority priority, Guid aircraftId, Guid clientId, string description, decimal estimatedHours, DateTime estimatedEndDate, decimal aircraftHoursAtStart, Guid createdById) sin fecha de ingreso
    public static WorkOrder Create(
        string number, WorkOrderType type, WorkOrderPriority priority,
        Guid aircraftId, Guid clientId, string description,
        decimal estimatedHours, DateTime intakeDate, DateTime estimatedEndDate,
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
            IntakeDate = intakeDate,
            EstimatedEndDate = estimatedEndDate,
            AircraftHoursAtStart = aircraftHoursAtStart,
            CreatedById = createdById
        };

        workOrder.AddDomainEvent(new WorkOrderCreatedEvent(workOrder.Id, number, createdById));
        return workOrder;
    }

    // Cambio de estado validado; el cierre exige tareas e inspeccion aprobadas
    public void TransitionTo(WorkOrderStatus newStatus, Guid userId, string userName, string userRole)
    {
        if (!ValidTransitions[Status].Contains(newStatus))
            throw new InvalidStatusTransitionException(Status.ToString(), newStatus.ToString());

        if (newStatus == WorkOrderStatus.Finalizada)
        {
            EnsureAllInspectionsCompleted();
            if (!Inspections.Any(i => i.Status == InspectionStatus.Aprobada))
                throw new DomainException("No se puede finalizar la OT sin una inspección aprobada.");
        }

        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        if (newStatus == WorkOrderStatus.EnProceso && !StartDate.HasValue)
            StartDate = DateTime.UtcNow;

        if (newStatus == WorkOrderStatus.Finalizada)
            CompletedDate = DateTime.UtcNow;

        var timelineEntry = WorkOrderTimeline.Create(
            Id, $"Estado: {oldStatus} → {newStatus}",
            $"Estado cambiado de '{oldStatus}' a '{newStatus}'",
            userId, userName, userRole);

        Timeline.Add(timelineEntry);
    }

    // Inspeccion abierta de la OT o una nueva si no hay ninguna en curso
    public (Inspection Inspection, bool IsNew) OpenInspection(decimal aircraftHours)
    {
        var open = Inspections.FirstOrDefault(i => i.IsOpen);
        if (open is not null) return (open, false);

        var inspection = Inspection.Create(Id, Type.ToString(), AircraftId, aircraftHours, DateTime.Today);
        Inspections.Add(inspection);
        return (inspection, true);
    }

    // Marca como inspeccionadas las tareas que lo requieren
    public void MarkInspectionTasksInspected(Guid inspectorId)
    {
        foreach (var task in Tasks.Where(t => t.RequiresInspection && t.InspectedAt is null))
            task.MarkInspected(inspectorId);
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

    // Edicion de datos generales; no aplica a OT cerradas
    public void Update(string description, decimal estimatedHours, DateTime intakeDate, DateTime estimatedEndDate, WorkOrderPriority priority)
    {
        EnsureEditable();
        Description = description.Trim();
        EstimatedHours = estimatedHours;
        IntakeDate = intakeDate;
        EstimatedEndDate = estimatedEndDate;
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsClosed => Status is WorkOrderStatus.Finalizada or WorkOrderStatus.Cancelada;

    // Estados a los que puede pasar la OT desde el actual
    public IReadOnlyList<WorkOrderStatus> GetAllowedTransitions() => ValidTransitions[Status];

    // Rechaza cambios sobre una OT finalizada o cancelada
    public void EnsureEditable()
    {
        if (IsClosed)
            throw new DomainException($"La OT está {Status}: no admite cambios.");
    }

    // Horas reales de la OT como suma de las horas reales de sus tareas
    public void RecalculateActualHours()
    {
        ActualHours = Tasks.Sum(t => t.ActualHours);
        UpdatedAt = DateTime.UtcNow;
    }
}
