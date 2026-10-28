using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.WorkOrders.Commands.Update;

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
