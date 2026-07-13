using SIGMA.Domain.Enums;

namespace SIGMA.Application.Appointments.DTOs;

// DTO de turno para exponer en la API — resuelve la matricula real si la aeronave esta registrada
public class AppointmentDto
{
    public Guid Id { get; init; }
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public Guid? AircraftId { get; init; }
    public string? AircraftRegistration { get; init; }
    public WorkOrderType RequestedType { get; init; }
    public DateTime ScheduledDate { get; init; }
    public AppointmentStatus Status { get; init; }
    public string? Notes { get; init; }
    public Guid? ConvertedWorkOrderId { get; init; }
    public DateTime CreatedAt { get; init; }
}
