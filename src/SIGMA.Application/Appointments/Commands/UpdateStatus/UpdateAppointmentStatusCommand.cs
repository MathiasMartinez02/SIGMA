using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Appointments.Commands.UpdateStatus;

// Comando para confirmar o cancelar un turno (la conversion a OT es una accion separada)
public record UpdateAppointmentStatusCommand(Guid Id, AppointmentStatus NewStatus) : IRequest<Result>;
