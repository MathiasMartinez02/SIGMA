using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;

namespace SIGMA.Domain.Entities;

public class ChecklistItem : BaseEntity
{
    public Guid SectionId { get; private set; }
    public ChecklistSection Section { get; private set; } = null!;
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Reference { get; private set; }
    public int OrderIndex { get; private set; }
    public ChecklistItemStatus Status { get; private set; } = ChecklistItemStatus.Pendiente;
    public string? Observations { get; private set; }
    public Guid? CheckedById { get; private set; }
    public DateTime? CheckedAt { get; private set; }
    public bool RequiresPhoto { get; private set; }
    public string? PhotoUrl { get; private set; }

    private ChecklistItem() { }

    public static ChecklistItem Create(Guid sectionId, string code, string description,
        int orderIndex, string? reference = null, bool requiresPhoto = false) =>
        new()
        {
            SectionId = sectionId, Code = code, Description = description,
            OrderIndex = orderIndex, Reference = reference, RequiresPhoto = requiresPhoto
        };

    public void Check(ChecklistItemStatus status, string? observations, Guid checkedById, string? photoUrl = null)
    {
        Status = status;
        Observations = observations;
        CheckedById = checkedById;
        CheckedAt = DateTime.UtcNow;
        if (photoUrl is not null) PhotoUrl = photoUrl;
    }
}
