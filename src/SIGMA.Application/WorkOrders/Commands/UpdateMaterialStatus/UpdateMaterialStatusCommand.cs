using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.WorkOrders.Commands.UpdateMaterialStatus;

// Comando para marcar un material de OT como Entregado o NoDisponible (Fase 8)
public record UpdateMaterialStatusCommand(
    Guid WorkOrderId,
    Guid MaterialId,
    WorkOrderMaterialStatus Status
) : IRequest<Result>;
