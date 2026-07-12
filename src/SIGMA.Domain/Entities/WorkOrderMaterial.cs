using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;

namespace SIGMA.Domain.Entities;

public class WorkOrderMaterial : BaseEntity
{
    public Guid WorkOrderId { get; private set; }
    public WorkOrder WorkOrder { get; private set; } = null!;
    public string PartNumber { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public DateTime RequestedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; private set; }
    public WorkOrderMaterialStatus Status { get; private set; } = WorkOrderMaterialStatus.Pendiente;

    private WorkOrderMaterial() { }

    public static WorkOrderMaterial Create(Guid workOrderId, string partNumber, string description,
        decimal quantity, string unit) =>
        new() { WorkOrderId = workOrderId, PartNumber = partNumber, Description = description, Quantity = quantity, Unit = unit };

    public void MarkDelivered()
    {
        Status = WorkOrderMaterialStatus.Entregado;
        DeliveredAt = DateTime.UtcNow;
    }

    public void MarkUnavailable() => Status = WorkOrderMaterialStatus.NoDisponible;
}
