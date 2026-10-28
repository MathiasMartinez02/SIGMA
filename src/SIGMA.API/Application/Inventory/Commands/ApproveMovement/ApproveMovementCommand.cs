using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Inventory.Commands.ApproveMovement;

// Comando para aprobar un movimiento de salida de inventario, identificado por ItemId y MovementId
public record ApproveMovementCommand(Guid ItemId, Guid MovementId) : IRequest<Result>;
