using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.WorkOrders.Commands.AddMechanic;

public class AddMechanicCommandHandler : IRequestHandler<AddMechanicCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AddMechanicCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result> Handle(AddMechanicCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _context.WorkOrders
            .FirstOrDefaultAsync(w => w.Id == request.WorkOrderId, cancellationToken);

        if (workOrder is null) return Result.Failure("Orden de trabajo no encontrada.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return Result.Failure("Usuario no encontrado.");

        var alreadyAssigned = await _context.AssignedMechanics
            .AnyAsync(am => am.WorkOrderId == request.WorkOrderId && am.UserId == request.UserId, cancellationToken);

        if (alreadyAssigned) return Result.Failure("El mecánico ya está asignado a esta orden de trabajo.");

        var mechanic = AssignedMechanic.Create(request.WorkOrderId, request.UserId, user.Role.ToString());
        _context.AssignedMechanics.Add(mechanic);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
