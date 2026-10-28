using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.WorkOrders.Commands.RemoveMechanic;

public record RemoveMechanicCommand(Guid WorkOrderId, Guid UserId) : IRequest<Result>;
