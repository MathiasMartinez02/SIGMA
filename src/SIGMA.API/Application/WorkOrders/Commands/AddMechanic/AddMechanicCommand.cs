using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.WorkOrders.Commands.AddMechanic;

public record AddMechanicCommand(Guid WorkOrderId, Guid UserId) : IRequest<Result>;
