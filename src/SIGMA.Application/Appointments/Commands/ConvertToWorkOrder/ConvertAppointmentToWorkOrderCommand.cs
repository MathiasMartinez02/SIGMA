using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;

namespace SIGMA.Application.Appointments.Commands.ConvertToWorkOrder;

// Comando para convertir un turno confirmado en una orden de trabajo
public record ConvertAppointmentToWorkOrderCommand(Guid AppointmentId) : IRequest<Result<WorkOrderDto>>;
