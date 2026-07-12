using SIGMA.Domain.Enums;

namespace SIGMA.Application.Inventory.DTOs;

public class InventoryItemDto
{
    public Guid Id { get; init; }
    public string PartNumber { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public InventoryCategory Category { get; init; }
    public string Manufacturer { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public decimal CurrentStock { get; init; }
    public decimal MinimumStock { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal UnitCost { get; init; }
    public InventoryStatus Status { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public bool CertificationRequired { get; init; }
    public string? CertificateNumber { get; init; }
    public DateTime? LastMovementDate { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class InventoryMovementDto
{
    public Guid Id { get; init; }
    public MovementType Type { get; init; }
    public decimal Quantity { get; init; }
    public decimal PreviousStock { get; init; }
    public decimal NewStock { get; init; }
    public Guid? WorkOrderId { get; init; }
    public string Reason { get; init; } = string.Empty;
    public Guid PerformedById { get; init; }
    public string PerformedByName { get; init; } = string.Empty;
    public DateTime PerformedAt { get; init; }
}
