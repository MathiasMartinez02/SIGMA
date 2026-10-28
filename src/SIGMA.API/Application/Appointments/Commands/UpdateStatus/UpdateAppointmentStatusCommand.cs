using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.Appointments.Commands.UpdateStatus;

// Comando para confirmar o cancelar un turno (la conversion a OT es una accion separada)
public record UpdateAppointmentStatusCommand(Guid Id, AppointmentStatus NewStatus) : IRequest<Result>;
