using SIGMA.Domain.Enums;

namespace SIGMA.Application.Budgets.DTOs;

// DTO de presupuesto para exponer en la API — Status ya viene resuelto con GetEffectiveStatus()
// (si esta Pendiente y vencio, se expone como Vencido sin que se haya persistido ese cambio)
public class BudgetDto
{
    public Guid Id { get; init; }
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public Guid? AircraftId { get; init; }
    public string? AircraftRegistration { get; init; }
    public Guid? AppointmentId { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime ValidUntil { get; init; }
    public BudgetStatus Status { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
}
