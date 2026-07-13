using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.WorkOrders.Commands.Update;

// MODIFICADO: agrega IntakeDate para permitir editar la fecha de ingreso de la OT
// ANTERIOR: UpdateWorkOrderCommand(Guid Id, string Description, decimal EstimatedHours, DateTime EstimatedEndDate, WorkOrderPriority Priority) sin fecha de ingreso
public record UpdateWorkOrderCommand(
    Guid Id,
    string Description,
    decimal EstimatedHours,
    DateTime IntakeDate,
    DateTime EstimatedEndDate,
    WorkOrderPriority Priority
) : IRequest<Result>;
