using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.WorkOrders.Commands.RemoveMechanic;

public class RemoveMechanicCommandHandler : IRequestHandler<RemoveMechanicCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public RemoveMechanicCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result> Handle(RemoveMechanicCommand request, CancellationToken cancellationToken)
    {
        var mechanic = await _context.AssignedMechanics
            .FirstOrDefaultAsync(am => am.WorkOrderId == request.WorkOrderId && am.UserId == request.UserId, cancellationToken);

        if (mechanic is null)
            return Result.Failure("El mecánico no está asignado a esta orden de trabajo.");

        _context.AssignedMechanics.Remove(mechanic);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
