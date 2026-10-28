using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;

namespace SIGMA.API.Appointments.Commands.ConvertToWorkOrder;

// Comando para convertir un turno confirmado en una orden de trabajo
public record ConvertAppointmentToWorkOrderCommand(Guid AppointmentId) : IRequest<Result<WorkOrderDto>>;
