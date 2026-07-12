using SIGMA.Domain.Common;

namespace SIGMA.Domain.Entities;

public class ChecklistSection : BaseEntity
{
    public Guid InspectionId { get; private set; }
    public Inspection Inspection { get; private set; } = null!;
    public string Title { get; private set; } = string.Empty;
    public int OrderIndex { get; private set; }

    public ICollection<ChecklistItem> Items { get; private set; } = [];

    private ChecklistSection() { }

    public static ChecklistSection Create(Guid inspectionId, string title, int orderIndex) =>
        new() { InspectionId = inspectionId, Title = title, OrderIndex = orderIndex };
}
