namespace SIGMA.Domain.Entities;

public class AssignedMechanic
{
    public Guid WorkOrderId { get; private set; }
    public WorkOrder WorkOrder { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Role { get; private set; } = "mecanico";
    public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;

    private AssignedMechanic() { }

    public static AssignedMechanic Create(Guid workOrderId, Guid userId, string role = "mecanico") =>
        new() { WorkOrderId = workOrderId, UserId = userId, Role = role };
}
