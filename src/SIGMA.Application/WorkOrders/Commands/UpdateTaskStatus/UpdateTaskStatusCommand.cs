using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.WorkOrders.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand(
    Guid WorkOrderId,
    Guid TaskId,
    WorkOrderTaskStatus Status,
    string? Observations
) : IRequest<Result>;
