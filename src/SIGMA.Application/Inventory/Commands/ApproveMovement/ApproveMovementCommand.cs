using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Inventory.Commands.ApproveMovement;

// Comando para aprobar un movimiento de salida de inventario, identificado por ItemId y MovementId
public record ApproveMovementCommand(Guid ItemId, Guid MovementId) : IRequest<Result>;
