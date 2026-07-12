using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Clients.Commands.Update;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateClientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (client is null)
            return Result.Failure("El cliente no fue encontrado.");

        var taxIdConflict = await _context.Clients
            .AnyAsync(c => c.TaxId == request.TaxId && c.Id != request.Id && !c.IsDeleted, cancellationToken);

        if (taxIdConflict)
            return Result.Failure($"Ya existe otro cliente con el CUIT '{request.TaxId}'.");

        client.Update(
            request.Name,
            request.BusinessName,
            request.TaxId,
            request.Email,
            request.Phone,
            request.Address,
            request.City,
            request.Province,
            request.ContactPerson,
            request.ContactPhone);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
