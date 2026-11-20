using SIGMA.API.Checklists;
using SIGMA.API.Common;
using SIGMA.API.Enums;
using SIGMA.API.Exceptions;

namespace SIGMA.API.Entities;

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

    public bool IsOpen => Status is InspectionStatus.Pendiente or InspectionStatus.EnProceso or InspectionStatus.Completada;

    // Alta con el checklist de la plantilla del tipo de inspeccion
    public static Inspection Create(Guid workOrderId, string type, Guid aircraftId,
        decimal aircraftHours, DateTime scheduledDate, Guid? inspectorId = null)
    {
        var inspection = new Inspection
        {
            WorkOrderId = workOrderId, Type = type, AircraftId = aircraftId,
            AircraftHours = aircraftHours, ScheduledDate = scheduledDate,
            InspectorId = inspectorId
        };
        inspection.EnsureChecklist();
        return inspection;
    }

    // Genera el checklist de la plantilla si todavia no tiene
    public void EnsureChecklist()
    {
        if (ChecklistSections.Count != 0) return;

        var sectionIndex = 0;
        foreach (var sectionTemplate in ChecklistTemplates.For(Type))
        {
            var section = ChecklistSection.Create(Id, sectionTemplate.Title, ++sectionIndex);
            var itemIndex = 0;
            foreach (var itemTemplate in sectionTemplate.Items)
                section.Items.Add(ChecklistItem.Create(section.Id, itemTemplate.Code, itemTemplate.Description,
                    ++itemIndex, itemTemplate.Reference));
            ChecklistSections.Add(section);
        }
    }

    // Inicio de la inspeccion por el inspector que la toma
    public void Start(Guid inspectorId)
    {
        if (Status != InspectionStatus.Pendiente)
            throw new DomainException("Solo se puede iniciar una inspección en estado Pendiente.");
        EnsureChecklist();
        InspectorId = inspectorId;
        Status = InspectionStatus.EnProceso;
        StartedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Control de un item del checklist durante la inspeccion
    public void CheckItem(Guid itemId, ChecklistItemStatus status, string? observations, Guid checkedById)
    {
        if (Status != InspectionStatus.EnProceso)
            throw new DomainException("Solo se puede controlar el checklist de una inspección en proceso.");
        if (status == ChecklistItemStatus.Pendiente)
            throw new DomainException("Indicá si el ítem es Conforme, No conforme o No aplica.");
        if (status == ChecklistItemStatus.NoConforme && string.IsNullOrWhiteSpace(observations))
            throw new DomainException("Un ítem No conforme requiere una observación.");

        var item = ChecklistSections.SelectMany(s => s.Items).FirstOrDefault(i => i.Id == itemId)
            ?? throw new DomainException("El ítem del checklist no fue encontrado en esta inspección.");

        item.Check(status, observations, checkedById);
        UpdatedAt = DateTime.UtcNow;
    }

    // Cierre del checklist; exige todos los items controlados
    public void Complete(string? observations = null)
    {
        if (Status != InspectionStatus.EnProceso)
            throw new DomainException("Solo se puede completar una inspección en proceso.");

        var pending = ChecklistSections.SelectMany(s => s.Items).Count(i => i.Status == ChecklistItemStatus.Pendiente);
        if (pending != 0)
            throw new DomainException($"Quedan {pending} ítem(s) del checklist sin controlar.");

        Status = InspectionStatus.Completada;
        CompletedAt = DateTime.UtcNow;
        Observations = observations;
        UpdatedAt = DateTime.UtcNow;
    }

    // Aprobacion de una inspeccion completada sin no conformidades
    public void Approve(Guid approvedById, string? observations = null)
    {
        if (Status != InspectionStatus.Completada)
            throw new DomainException("Solo se puede aprobar una inspección completada.");
        if (ChecklistSections.SelectMany(s => s.Items).Any(i => i.Status == ChecklistItemStatus.NoConforme))
            throw new DomainException("La inspección tiene ítems No conformes: debe rechazarse.");

        Status = InspectionStatus.Aprobada;
        ApprovedAt = DateTime.UtcNow;
        ApprovedById = approvedById;
        OverallResult = "Aprobada";
        if (observations is not null) Observations = observations;
        UpdatedAt = DateTime.UtcNow;
    }

    // Rechazo con motivo de una inspeccion en curso o completada
    public void Reject(string rejectionReason, Guid rejectedById)
    {
        if (Status != InspectionStatus.EnProceso && Status != InspectionStatus.Completada)
            throw new DomainException("Solo se puede rechazar una inspección en proceso o completada.");
        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new DomainException("El motivo de rechazo es requerido.");

        Status = InspectionStatus.Rechazada;
        RejectionReason = rejectionReason.Trim();
        OverallResult = "Rechazada";
        ApprovedById = rejectedById;
        UpdatedAt = DateTime.UtcNow;
    }
}
