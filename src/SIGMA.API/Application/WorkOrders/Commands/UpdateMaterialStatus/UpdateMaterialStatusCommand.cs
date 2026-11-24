using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.WorkOrders.Commands.UpdateMaterialStatus;

public record UpdateMaterialStatusCommand(
    Guid WorkOrderId,
    Guid MaterialId,
    WorkOrderMaterialStatus Status
) : IRequest<Result>;
