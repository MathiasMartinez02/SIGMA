using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.Appointments.Commands.Update;

public record UpdateAppointmentCommand(
    Guid Id,
    Guid? AircraftId,
    string? AircraftRegistrationHint,
    WorkOrderType RequestedType,
    DateTime ScheduledDate,
    string? Notes
) : IRequest<Result>;
