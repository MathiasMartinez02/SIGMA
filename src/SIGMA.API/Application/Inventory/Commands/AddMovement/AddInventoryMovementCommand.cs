using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Inventory.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.Inventory.Commands.AddMovement;

public record AddInventoryMovementCommand(
    Guid ItemId,
    MovementType Type,
    decimal Quantity,
    Guid? WorkOrderId,
    string Reason
) : IRequest<Result<InventoryMovementDto>>;
