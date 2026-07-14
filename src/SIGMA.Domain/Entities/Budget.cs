using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Domain.Entities;

// Presupuesto para un cliente (MVP de la Fase 6 Administrativo), valido por 30 dias desde su creacion.
// Puede originarse a partir de un turno solicitado (Appointment) sin que este dependa de Budget para nada
// (relacion desacoplada: solo Budget conoce el AppointmentId, Appointment no se modifico).
public class Budget : AuditableEntity
{
    private const int ValidityDays = 30;

    public Guid ClientId { get; private set; }
    public Client Client { get; private set; } = null!;
    public Guid? AircraftId { get; private set; }
    public Aircraft? Aircraft { get; private set; }
    public Guid? AppointmentId { get; private set; }
    public Appointment? Appointment { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public DateTime ValidUntil { get; private set; }
    public BudgetStatus Status { get; private set; } = BudgetStatus.Pendiente;
    public string? Notes { get; private set; }

    private Budget() { }

    // Crea un presupuesto nuevo en estado Pendiente, con vencimiento automatico a 30 dias desde ahora
    public static Budget Create(
        Guid clientId, Guid? aircraftId, Guid? appointmentId,
        string description, decimal amount, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La descripción del presupuesto es requerida.");
        if (amount <= 0)
            throw new DomainException("El monto del presupuesto debe ser mayor a cero.");

        return new Budget
        {
            ClientId = clientId,
            AircraftId = aircraftId,
            AppointmentId = appointmentId,
            Description = description.Trim(),
            Amount = amount,
            ValidUntil = DateTime.UtcNow.AddDays(ValidityDays),
            Notes = notes?.Trim()
        };
    }

    // Calcula el estado real del presupuesto: si sigue Pendiente pero ya paso su fecha de validez, se
    // considera Vencido. Es una propiedad calculada al vuelo (no persistida, no requiere job periodico),
    // mismo criterio de "vencimiento por fecha" que InventoryItem.RecalculateStatus() usa para ExpiryDate,
    // salvo que aca no se persiste el cambio porque no hay ninguna mutacion que lo dispare.
    public BudgetStatus GetEffectiveStatus()
    {
        if (Status == BudgetStatus.Pendiente && ValidUntil < DateTime.UtcNow)
            return BudgetStatus.Vencido;
        return Status;
    }

    // Acepta el presupuesto, siempre que su estado efectivo actual sea Pendiente (no Vencido)
    public void Accept()
    {
        if (GetEffectiveStatus() != BudgetStatus.Pendiente)
            throw new DomainException("Solo se puede aceptar un presupuesto en estado Pendiente.");
        Status = BudgetStatus.Aceptado;
        UpdatedAt = DateTime.UtcNow;
    }

    // Rechaza el presupuesto, siempre que su estado efectivo actual sea Pendiente (no Vencido)
    public void Reject()
    {
        if (GetEffectiveStatus() != BudgetStatus.Pendiente)
            throw new DomainException("Solo se puede rechazar un presupuesto en estado Pendiente.");
        Status = BudgetStatus.Rechazado;
        UpdatedAt = DateTime.UtcNow;
    }
}
