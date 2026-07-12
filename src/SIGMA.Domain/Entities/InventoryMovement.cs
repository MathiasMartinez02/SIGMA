using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;

namespace SIGMA.Domain.Entities;

public class InventoryMovement : BaseEntity
{
    public Guid ItemId { get; private set; }
    public InventoryItem Item { get; private set; } = null!;
    public MovementType Type { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal PreviousStock { get; private set; }
    public decimal NewStock { get; private set; }
    public Guid? WorkOrderId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public Guid PerformedById { get; private set; }
    public User PerformedBy { get; private set; } = null!;
    public Guid? ApprovedById { get; private set; }
    public DateTime PerformedAt { get; private set; } = DateTime.UtcNow;

    private InventoryMovement() { }

    public static InventoryMovement Create(
        Guid itemId, MovementType type, decimal quantity,
        decimal previousStock, decimal newStock, string reason,
        Guid performedById, Guid? workOrderId = null, Guid? approvedById = null) =>
        new()
        {
            ItemId = itemId, Type = type, Quantity = quantity,
            PreviousStock = previousStock, NewStock = newStock,
            Reason = reason, PerformedById = performedById,
            WorkOrderId = workOrderId, ApprovedById = approvedById
        };
}
