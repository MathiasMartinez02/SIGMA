using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.WorkOrders.Commands.Update;

public record UpdateWorkOrderCommand(
    Guid Id,
    string Description,
    decimal EstimatedHours,
    DateTime IntakeDate,
    DateTime EstimatedEndDate,
    WorkOrderPriority Priority
) : IRequest<Result>;
