using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.WorkOrders.Commands.UpdateMaterialStatus;

// Comando para marcar un material de OT como Entregado o NoDisponible (Fase 8)
public record UpdateMaterialStatusCommand(
    Guid WorkOrderId,
    Guid MaterialId,
    WorkOrderMaterialStatus Status
) : IRequest<Result>;
