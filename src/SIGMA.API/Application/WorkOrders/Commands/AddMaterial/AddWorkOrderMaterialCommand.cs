using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;

namespace SIGMA.API.WorkOrders.Commands.AddMaterial;

public record AddWorkOrderMaterialCommand(
    Guid WorkOrderId,
    string PartNumber,
    string Description,
    decimal Quantity,
    string Unit
) : IRequest<Result<WorkOrderMaterialDto>>;
