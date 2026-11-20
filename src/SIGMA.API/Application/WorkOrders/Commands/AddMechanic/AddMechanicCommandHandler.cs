using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Entities;
using SIGMA.API.WorkOrders.Services;
using SIGMA.API.Enums;

namespace SIGMA.API.WorkOrders.Commands.AddMechanic;

public class AddMechanicCommandHandler : IRequestHandler<AddMechanicCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AddMechanicCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result> Handle(AddMechanicCommand request, CancellationToken cancellationToken)
    {
        var (workOrder, error) = await EditableWorkOrder.FindAsync(_context, request.WorkOrderId, cancellationToken);
        if (workOrder is null) return Result.Failure(error!);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result.Failure("Usuario no encontrado.");
        if (!user.IsActive) return Result.Failure("El usuario está desactivado.");
        if (user.Role is not (UserRole.Mecanico or UserRole.JefeTaller))
            return Result.Failure("Solo se pueden asignar mecánicos o jefes de taller a una OT.");

        var alreadyAssigned = await _context.AssignedMechanics
            .AnyAsync(am => am.WorkOrderId == request.WorkOrderId && am.UserId == request.UserId, cancellationToken);

        if (alreadyAssigned) return Result.Failure("El mecánico ya está asignado a esta orden de trabajo.");

        var mechanic = AssignedMechanic.Create(request.WorkOrderId, request.UserId, user.Role.ToString());
        _context.AssignedMechanics.Add(mechanic);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
