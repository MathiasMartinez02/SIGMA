using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Inventory.Commands.Delete;

// Maneja la baja de un item de inventario: bloquea si tiene stock actual y aplica soft delete preservando el historial de movimientos
public class DeleteInventoryItemCommandHandler : IRequestHandler<DeleteInventoryItemCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteInventoryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

        if (item is null)
            return Result.Failure("El ítem de inventario no fue encontrado.");

        if (item.CurrentStock > 0)
            return Result.Failure("No se puede dar de baja el ítem porque todavía tiene stock. Retire el stock antes de eliminarlo.");

        item.SoftDelete();
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
