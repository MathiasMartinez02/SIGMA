using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Domain.Entities;

public class InventoryItem : AuditableEntity
{
    public string PartNumber { get; private set; } = string.Empty;
    public string AltPartNumbers { get; private set; } = "[]";
    public string Description { get; private set; } = string.Empty;
    public InventoryCategory Category { get; private set; }
    public string Manufacturer { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public decimal CurrentStock { get; private set; }
    public decimal MinimumStock { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal UnitCost { get; private set; }
    public InventoryStatus Status { get; private set; } = InventoryStatus.Disponible;
    public DateTime? ExpiryDate { get; private set; }
    public bool CertificationRequired { get; private set; }
    public string? CertificateNumber { get; private set; }
    public DateTime? LastMovementDate { get; private set; }

    public ICollection<InventoryMovement> Movements { get; private set; } = [];

    private InventoryItem() { }

    public static InventoryItem Create(
        string partNumber, string description, InventoryCategory category,
        string manufacturer, string location, decimal minimumStock,
        string unit, decimal unitCost, bool certificationRequired = false,
        DateTime? expiryDate = null, string? altPartNumbers = null) =>
        new()
        {
            PartNumber = partNumber.Trim().ToUpperInvariant(),
            Description = description.Trim(),
            Category = category,
            Manufacturer = manufacturer.Trim(),
            Location = location.Trim(),
            MinimumStock = minimumStock,
            Unit = unit,
            UnitCost = unitCost,
            CertificationRequired = certificationRequired,
            ExpiryDate = expiryDate,
            AltPartNumbers = altPartNumbers ?? "[]"
        };

    public void ApplyMovement(decimal quantity, MovementType movementType)
    {
        var previousStock = CurrentStock;

        CurrentStock = movementType switch
        {
            MovementType.Entrada or MovementType.Devolucion => CurrentStock + quantity,
            MovementType.Salida or MovementType.Reserva => CurrentStock - quantity,
            MovementType.Ajuste => quantity,
            _ => CurrentStock
        };

        if (CurrentStock < 0)
            throw new DomainException($"Stock insuficiente. Stock actual: {previousStock}, solicitado: {quantity}.");

        RecalculateStatus();
        LastMovementDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecalculateStatus()
    {
        if (ExpiryDate.HasValue && ExpiryDate.Value.Date < DateTime.UtcNow.Date)
            Status = InventoryStatus.Vencido;
        else if (CurrentStock <= 0)
            Status = InventoryStatus.SinStock;
        else if (CurrentStock <= MinimumStock)
            Status = InventoryStatus.BajoStock;
        else
            Status = InventoryStatus.Disponible;
    }

    public void Update(string description, string location, decimal minimumStock,
        decimal unitCost, string? certificateNumber, DateTime? expiryDate)
    {
        Description = description.Trim();
        Location = location.Trim();
        MinimumStock = minimumStock;
        UnitCost = unitCost;
        CertificateNumber = certificateNumber;
        ExpiryDate = expiryDate;
        RecalculateStatus();
        UpdatedAt = DateTime.UtcNow;
    }
}
