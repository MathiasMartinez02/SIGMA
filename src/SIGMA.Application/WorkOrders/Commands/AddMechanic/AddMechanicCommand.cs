using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.WorkOrders.Commands.AddMechanic;

public record AddMechanicCommand(Guid WorkOrderId, Guid UserId) : IRequest<Result>;
