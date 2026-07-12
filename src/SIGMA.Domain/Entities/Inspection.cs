using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Domain.Entities;

public class Inspection : AuditableEntity
{
    public Guid WorkOrderId { get; private set; }
    public WorkOrder WorkOrder { get; private set; } = null!;
    public string Type { get; private set; } = string.Empty;
    public InspectionStatus Status { get; private set; } = InspectionStatus.Pendiente;
    public Guid AircraftId { get; private set; }
    public Aircraft Aircraft { get; private set; } = null!;
    public decimal AircraftHours { get; private set; }
    public Guid? InspectorId { get; private set; }
    public User? Inspector { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public Guid? ApprovedById { get; private set; }
    public User? ApprovedBy { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? OverallResult { get; private set; }
    public string? Observations { get; private set; }

    public ICollection<ChecklistSection> ChecklistSections { get; private set; } = [];

    private Inspection() { }

    public static Inspection Create(Guid workOrderId, string type, Guid aircraftId,
        decimal aircraftHours, DateTime scheduledDate, Guid? inspectorId = null) =>
        new()
        {
            WorkOrderId = workOrderId, Type = type, AircraftId = aircraftId,
            AircraftHours = aircraftHours, ScheduledDate = scheduledDate,
            InspectorId = inspectorId
        };

    public void Start()
    {
        if (Status != InspectionStatus.Pendiente)
            throw new DomainException("Solo se puede iniciar una inspección en estado Pendiente.");
        Status = InspectionStatus.EnProceso;
        StartedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string? observations = null)
    {
        Status = InspectionStatus.Completada;
        CompletedAt = DateTime.UtcNow;
        Observations = observations;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(Guid approvedById, string? observations = null)
    {
        if (Status != InspectionStatus.Completada && Status != InspectionStatus.EnProceso)
            throw new DomainException("Solo se puede aprobar una inspección en proceso o completada.");

        Status = InspectionStatus.Aprobada;
        ApprovedAt = DateTime.UtcNow;
        ApprovedById = approvedById;
        OverallResult = "Aprobada";
        Observations = observations;
        CompletedAt ??= DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string rejectionReason, Guid rejectedById)
    {
        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new DomainException("El motivo de rechazo es requerido.");
        Status = InspectionStatus.Rechazada;
        RejectionReason = rejectionReason;
        OverallResult = "Rechazada";
        ApprovedById = rejectedById;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignInspector(Guid inspectorId)
    {
        InspectorId = inspectorId;
        UpdatedAt = DateTime.UtcNow;
    }
}
