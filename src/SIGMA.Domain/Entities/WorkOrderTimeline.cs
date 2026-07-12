using SIGMA.Domain.Common;

namespace SIGMA.Domain.Entities;

public class WorkOrderTimeline : BaseEntity
{
    public Guid WorkOrderId { get; private set; }
    public WorkOrder WorkOrder { get; private set; } = null!;
    public string Event { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string UserRole { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public string? Metadata { get; private set; }

    private WorkOrderTimeline() { }

    public static WorkOrderTimeline Create(
        Guid workOrderId, string @event, string description,
        Guid userId, string userName, string userRole, string? metadata = null) =>
        new()
        {
            WorkOrderId = workOrderId,
            Event = @event,
            Description = description,
            UserId = userId,
            UserName = userName,
            UserRole = userRole,
            Metadata = metadata
        };
}
