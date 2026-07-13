using MediatR;
using SIGMA.Application.Appointments.DTOs;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Appointments.Commands.Create;

// Comando para registrar un turno solicitado por un cliente, con aeronave opcional
public record CreateAppointmentCommand(
    Guid ClientId,
    Guid? AircraftId,
    string? AircraftRegistrationHint,
    WorkOrderType RequestedType,
    DateTime ScheduledDate,
    string? Notes
) : IRequest<Result<AppointmentDto>>;
