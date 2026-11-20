using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.WorkOrders.Commands.AssignTask;

public record AssignTaskCommand(Guid WorkOrderId, Guid TaskId, Guid? UserId) : IRequest<Result>;
