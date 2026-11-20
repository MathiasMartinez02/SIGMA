using SIGMA.API.Common;
using SIGMA.API.Enums;
using SIGMA.API.Exceptions;

namespace SIGMA.API.Entities;

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
    // Stock maximo recomendado para el item; nulo cuando no aplica limite superior
    public decimal? MaximumStock { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal UnitCost { get; private set; }
    public InventoryStatus Status { get; private set; } = InventoryStatus.Disponible;
    public DateTime? ExpiryDate { get; private set; }
    public bool CertificationRequired { get; private set; }
    public string? CertificateNumber { get; private set; }
    public DateTime? LastMovementDate { get; private set; }

    public ICollection<InventoryMovement> Movements { get; private set; } = [];

    private InventoryItem() { }

    // Alta de item con estado inicial segun stock y vencimiento
    public static InventoryItem Create(
        string partNumber, string description, InventoryCategory category,
        string manufacturer, string location, decimal minimumStock,
        string unit, decimal unitCost, bool certificationRequired = false,
        DateTime? expiryDate = null, string? altPartNumbers = null,
        decimal? maximumStock = null)
    {
        var item = new InventoryItem
        {
            PartNumber = partNumber.Trim().ToUpperInvariant(),
            Description = description.Trim(),
            Category = category,
            Manufacturer = manufacturer.Trim(),
            Location = location.Trim(),
            MinimumStock = minimumStock,
            MaximumStock = maximumStock,
            Unit = unit,
            UnitCost = unitCost,
            CertificationRequired = certificationRequired,
            ExpiryDate = expiryDate,
            AltPartNumbers = altPartNumbers ?? "[]"
        };
        item.RecalculateStatus();
        return item;
    }

    // Aplica un movimiento validando cantidad y stock resultante
    public void ApplyMovement(decimal quantity, MovementType movementType)
    {
        if (movementType == MovementType.Ajuste ? quantity < 0 : quantity <= 0)
            throw new DomainException("La cantidad del movimiento debe ser mayor a cero.");

        var newStock = movementType switch
        {
            MovementType.Entrada or MovementType.Devolucion => CurrentStock + quantity,
            MovementType.Salida or MovementType.Reserva => CurrentStock - quantity,
            MovementType.Ajuste => quantity,
            _ => CurrentStock
        };

        if (newStock < 0)
            throw new DomainException($"Stock insuficiente. Stock actual: {CurrentStock}, solicitado: {quantity}.");

        CurrentStock = newStock;
        RecalculateStatus();
        LastMovementDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // MODIFICADO: agrega el estado SobreStock cuando hay MaximumStock configurado y se supera, con prioridad menor a Vencido pero mayor al resto
    // ANTERIOR: solo evaluaba Vencido, SinStock, BajoStock o Disponible, sin considerar un limite maximo de stock
    public void RecalculateStatus()
    {
        if (ExpiryDate.HasValue && ExpiryDate.Value.Date < DateTime.UtcNow.Date)
            Status = InventoryStatus.Vencido;
        else if (MaximumStock.HasValue && CurrentStock > MaximumStock.Value)
            Status = InventoryStatus.SobreStock;
        else if (CurrentStock <= 0)
            Status = InventoryStatus.SinStock;
        else if (CurrentStock <= MinimumStock)
            Status = InventoryStatus.BajoStock;
        else
            Status = InventoryStatus.Disponible;
    }

    // MODIFICADO: agrega el parametro opcional maximumStock para poder editar el limite superior de stock
    // ANTERIOR: Update(string description, string location, decimal minimumStock, decimal unitCost, string? certificateNumber, DateTime? expiryDate) sin maximumStock
    public void Update(string description, string location, decimal minimumStock,
        decimal unitCost, string? certificateNumber, DateTime? expiryDate,
        decimal? maximumStock = null)
    {
        Description = description.Trim();
        Location = location.Trim();
        MinimumStock = minimumStock;
        MaximumStock = maximumStock;
        UnitCost = unitCost;
        CertificateNumber = certificateNumber;
        ExpiryDate = expiryDate;
        RecalculateStatus();
        UpdatedAt = DateTime.UtcNow;
    }
}
