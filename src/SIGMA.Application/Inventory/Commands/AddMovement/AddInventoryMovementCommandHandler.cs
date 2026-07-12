using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inventory.DTOs;
using SIGMA.Domain.Entities;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Inventory.Commands.AddMovement;

public class AddInventoryMovementCommandHandler : IRequestHandler<AddInventoryMovementCommand, Result<InventoryMovementDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AddInventoryMovementCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<InventoryMovementDto>> Handle(AddInventoryMovementCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && !i.IsDeleted, cancellationToken);

        if (item is null)
            return Result<InventoryMovementDto>.Failure("El ítem de inventario no fue encontrado.");

        if ((request.Type == MovementType.Salida || request.Type == MovementType.Reserva)
            && item.CurrentStock < request.Quantity)
        {
            return Result<InventoryMovementDto>.Failure(
                $"Stock insuficiente. Stock actual: {item.CurrentStock}, cantidad solicitada: {request.Quantity}.");
        }

        var previousStock = item.CurrentStock;

        try
        {
            item.ApplyMovement(request.Quantity, request.Type);
        }
        catch (Exception ex)
        {
            return Result<InventoryMovementDto>.Failure(ex.Message);
        }

        var newStock = item.CurrentStock;
        var userId = _currentUser.UserId!.Value;

        var movement = InventoryMovement.Create(
            request.ItemId,
            request.Type,
            request.Quantity,
            previousStock,
            newStock,
            request.Reason,
            userId,
            request.WorkOrderId);

        _context.InventoryMovements.Add(movement);
        await _context.SaveChangesAsync(cancellationToken);

        var performedBy = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        var dto = new InventoryMovementDto
        {
            Id = movement.Id,
            Type = movement.Type,
            Quantity = movement.Quantity,
            PreviousStock = movement.PreviousStock,
            NewStock = movement.NewStock,
            WorkOrderId = movement.WorkOrderId,
            Reason = movement.Reason,
            PerformedById = movement.PerformedById,
            PerformedByName = performedBy is not null
                ? $"{performedBy.FirstName} {performedBy.LastName}"
                : _currentUser.FullName ?? string.Empty,
            PerformedAt = movement.PerformedAt
        };

        return Result<InventoryMovementDto>.Success(dto);
    }
}
