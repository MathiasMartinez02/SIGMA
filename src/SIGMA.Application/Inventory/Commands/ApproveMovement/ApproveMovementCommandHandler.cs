using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Application.Inventory.Commands.ApproveMovement;

// Maneja la aprobacion de un movimiento de salida de inventario, asignando el usuario actual como aprobador
public class ApproveMovementCommandHandler : IRequestHandler<ApproveMovementCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ApproveMovementCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ApproveMovementCommand request, CancellationToken cancellationToken)
    {
        var movement = await _context.InventoryMovements
            .FirstOrDefaultAsync(m => m.Id == request.MovementId && m.ItemId == request.ItemId, cancellationToken);

        if (movement is null)
            return Result.Failure("El movimiento de inventario no fue encontrado.");

        if (_currentUser.UserId is null)
            return Result.Failure("Usuario actual no identificado.");

        try
        {
            movement.Approve(_currentUser.UserId.Value);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
