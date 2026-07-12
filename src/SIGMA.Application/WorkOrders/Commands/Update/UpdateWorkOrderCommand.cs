using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.WorkOrders.Commands.Update;

public record UpdateWorkOrderCommand(
    Guid Id,
    string Description,
    decimal EstimatedHours,
    DateTime EstimatedEndDate,
    WorkOrderPriority Priority
) : IRequest<Result>;
