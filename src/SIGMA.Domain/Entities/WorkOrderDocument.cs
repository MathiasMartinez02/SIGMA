using SIGMA.Domain.Common;

namespace SIGMA.Domain.Entities;

public class WorkOrderDocument : BaseEntity
{
    public Guid WorkOrderId { get; private set; }
    public WorkOrder WorkOrder { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string FileUrl { get; private set; } = string.Empty;
    public Guid UploadedById { get; private set; }
    public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;

    private WorkOrderDocument() { }

    public static WorkOrderDocument Create(Guid workOrderId, string name, string type, string fileUrl, Guid uploadedById) =>
        new() { WorkOrderId = workOrderId, Name = name, Type = type, FileUrl = fileUrl, UploadedById = uploadedById };
}
