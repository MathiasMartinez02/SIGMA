using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;

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
    // Navegacion opcional hacia la orden de trabajo relacionada, usada para trazabilidad repuesto-aeronave
    public WorkOrder? WorkOrder { get; private set; }
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

    // Aprueba un movimiento de salida, seteando el usuario aprobador; falla si no es Salida o ya esta aprobado
    public void Approve(Guid approvedById)
    {
        if (Type != MovementType.Salida)
            throw new DomainException("Solo se pueden aprobar movimientos de salida.");

        if (ApprovedById.HasValue)
            throw new DomainException("El movimiento ya fue aprobado.");

        ApprovedById = approvedById;
    }
}
