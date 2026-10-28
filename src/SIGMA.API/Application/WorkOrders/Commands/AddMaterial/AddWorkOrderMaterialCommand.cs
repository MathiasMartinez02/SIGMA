using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;

namespace SIGMA.API.WorkOrders.Commands.AddMaterial;

// Comando para registrar un material/repuesto solicitado por una OT (Fase 8: WorkOrderMaterial no tenia capa de aplicacion)
public record AddWorkOrderMaterialCommand(
    Guid WorkOrderId,
    string PartNumber,
    string Description,
    decimal Quantity,
    string Unit
) : IRequest<Result<WorkOrderMaterialDto>>;
