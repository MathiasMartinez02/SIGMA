namespace SIGMA.Domain.Enums;

// Estados posibles de un presupuesto. Vencido no se persiste por un job: se calcula al vuelo
// (ver Budget.GetEffectiveStatus()) cuando un presupuesto Pendiente supera su fecha de validez.
public enum BudgetStatus
{
    Pendiente,
    Aceptado,
    Rechazado,
    Vencido
}
