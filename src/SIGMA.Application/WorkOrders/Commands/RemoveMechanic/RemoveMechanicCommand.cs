using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.WorkOrders.Commands.RemoveMechanic;

public record RemoveMechanicCommand(Guid WorkOrderId, Guid UserId) : IRequest<Result>;
