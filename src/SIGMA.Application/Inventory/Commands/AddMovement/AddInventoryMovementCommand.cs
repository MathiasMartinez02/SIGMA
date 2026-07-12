using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inventory.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Inventory.Commands.AddMovement;

public record AddInventoryMovementCommand(
    Guid ItemId,
    MovementType Type,
    decimal Quantity,
    Guid? WorkOrderId,
    string Reason
) : IRequest<Result<InventoryMovementDto>>;
