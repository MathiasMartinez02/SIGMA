using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.Services;

namespace SIGMA.API.WorkOrders.Commands.AssignTask;

public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AssignTaskCommandHandler(IApplicationDbContext context) => _context = context;

    // Asigna la tarea a un mecanico de la OT, o la deja sin responsable
    public async Task<Result> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var (workOrder, error) = await EditableWorkOrder.FindAsync(_context, request.WorkOrderId, cancellationToken,
            q => q.Include(w => w.Tasks).Include(w => w.AssignedMechanics));
        if (workOrder is null)
            return Result.Failure(error!);

        var task = workOrder.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task is null)
            return Result.Failure("Tarea no encontrada.");

        if (request.UserId is not null && workOrder.AssignedMechanics.All(m => m.UserId != request.UserId))
            return Result.Failure("Primero asigná el mecánico a la OT.");

        task.Assign(request.UserId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
