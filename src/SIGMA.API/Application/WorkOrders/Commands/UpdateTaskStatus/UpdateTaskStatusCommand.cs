using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.WorkOrders.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand(
    Guid WorkOrderId,
    Guid TaskId,
    WorkOrderTaskStatus Status,
    string? Observations
) : IRequest<Result>;
