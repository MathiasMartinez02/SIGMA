using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.WorkOrders.Commands.UpdateStatus;

public record UpdateWorkOrderStatusCommand(Guid WorkOrderId, WorkOrderStatus NewStatus) : IRequest<Result>;
