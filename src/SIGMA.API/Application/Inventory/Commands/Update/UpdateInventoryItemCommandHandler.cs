using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Inventory.Commands.Update;

public class UpdateInventoryItemCommandHandler : IRequestHandler<UpdateInventoryItemCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateInventoryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    // MODIFICADO: ahora tambien propaga MaximumStock al metodo de dominio Update
    // ANTERIOR: no enviaba MaximumStock porque la propiedad no existia en el comando
    public async Task<Result> Handle(UpdateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

        if (item is null)
            return Result.Failure("El ítem de inventario no fue encontrado.");

        item.Update(
            request.Description,
            request.Location,
            request.MinimumStock,
            request.UnitCost,
            request.CertificateNumber,
            request.ExpiryDate,
            request.MaximumStock);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
