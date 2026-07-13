namespace SIGMA.Domain.Enums;

public enum InventoryStatus
{
    Disponible,
    Reservado,
    BajoStock,
    SinStock,
    Vencido,
    // Estado cuando el stock actual supera el stock maximo configurado para el item
    SobreStock
}
