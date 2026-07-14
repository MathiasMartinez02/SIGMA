using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;

namespace SIGMA.Application.WorkOrders.Commands.AddMaterial;

// Comando para registrar un material/repuesto solicitado por una OT (Fase 8: WorkOrderMaterial no tenia capa de aplicacion)
public record AddWorkOrderMaterialCommand(
    Guid WorkOrderId,
    string PartNumber,
    string Description,
    decimal Quantity,
    string Unit
) : IRequest<Result<WorkOrderMaterialDto>>;
