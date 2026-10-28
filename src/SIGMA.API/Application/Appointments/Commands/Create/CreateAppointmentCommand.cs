using MediatR;
using SIGMA.API.Appointments.DTOs;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.Appointments.Commands.Create;

// Comando para registrar un turno solicitado por un cliente, con aeronave opcional
public record CreateAppointmentCommand(
    Guid ClientId,
    Guid? AircraftId,
    string? AircraftRegistrationHint,
    WorkOrderType RequestedType,
    DateTime ScheduledDate,
    string? Notes
) : IRequest<Result<AppointmentDto>>;
