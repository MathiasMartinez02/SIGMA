using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Clients.Commands.UpdateStatus;

// Maneja el comando de activar/desactivar cliente invocando los metodos de dominio Activate/Deactivate
public class UpdateClientStatusCommandHandler : IRequestHandler<UpdateClientStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateClientStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateClientStatusCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (client is null)
            return Result.Failure("El cliente no fue encontrado.");

        if (request.IsActive)
            client.Activate();
        else
            client.Deactivate();

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
