using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Domain.Entities;

// Turno solicitado por un cliente (telefono/WhatsApp) antes de que la aeronave llegue al taller y se cree la OT
public class Appointment : AuditableEntity
{
    public Guid ClientId { get; private set; }
    public Client Client { get; private set; } = null!;
    public Guid? AircraftId { get; private set; }
    public Aircraft? Aircraft { get; private set; }
    public string? AircraftRegistrationHint { get; private set; }
    public WorkOrderType RequestedType { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Solicitado;
    public string? Notes { get; private set; }
    public Guid? ConvertedWorkOrderId { get; private set; }

    private Appointment() { }

    // Crea un turno nuevo en estado Solicitado, con aeronave opcional si el cliente todavia no esta registrado
    public static Appointment Create(
        Guid clientId, Guid? aircraftId, string? aircraftRegistrationHint,
        WorkOrderType requestedType, DateTime scheduledDate, string? notes = null)
    {
        if (aircraftId is null && string.IsNullOrWhiteSpace(aircraftRegistrationHint))
            throw new DomainException("Debe indicar la aeronave registrada o una matrícula de referencia.");

        return new Appointment
        {
            ClientId = clientId,
            AircraftId = aircraftId,
            AircraftRegistrationHint = aircraftId is null ? aircraftRegistrationHint?.Trim() : null,
            RequestedType = requestedType,
            ScheduledDate = scheduledDate,
            Notes = notes?.Trim()
        };
    }

    // Confirma un turno que estaba en estado Solicitado
    public void Confirm()
    {
        if (Status != AppointmentStatus.Solicitado)
            throw new DomainException("Solo se puede confirmar un turno en estado Solicitado.");
        Status = AppointmentStatus.Confirmado;
        UpdatedAt = DateTime.UtcNow;
    }

    // Cancela el turno, siempre que no haya sido convertido ya en una OT
    public void Cancel()
    {
        if (Status == AppointmentStatus.ConvertidoOT)
            throw new DomainException("No se puede cancelar un turno que ya fue convertido en orden de trabajo.");
        Status = AppointmentStatus.Cancelado;
        UpdatedAt = DateTime.UtcNow;
    }

    // Marca el turno como convertido en OT, guardando el id de la orden de trabajo generada
    public void MarkConverted(Guid workOrderId)
    {
        if (Status != AppointmentStatus.Confirmado)
            throw new DomainException("Solo se puede convertir en orden de trabajo un turno Confirmado.");
        Status = AppointmentStatus.ConvertidoOT;
        ConvertedWorkOrderId = workOrderId;
        UpdatedAt = DateTime.UtcNow;
    }
}
