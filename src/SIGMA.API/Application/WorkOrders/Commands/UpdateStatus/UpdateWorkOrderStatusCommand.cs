using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.WorkOrders.Commands.UpdateStatus;

public record UpdateWorkOrderStatusCommand(Guid WorkOrderId, WorkOrderStatus NewStatus) : IRequest<Result>;
