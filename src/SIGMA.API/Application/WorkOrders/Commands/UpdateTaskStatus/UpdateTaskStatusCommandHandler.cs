using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Exceptions;
using SIGMA.API.WorkOrders.Services;

namespace SIGMA.API.WorkOrders.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateTaskStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    // Cambio de estado de tarea por quien gestiona OT o por el mecanico asignado; actualiza las horas de la OT
    public async Task<Result> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var (workOrder, error) = await EditableWorkOrder.FindAsync(_context, request.WorkOrderId, cancellationToken,
            q => q.Include(w => w.Tasks).Include(w => w.AssignedMechanics));
        if (workOrder is null)
            return Result.Failure(error!);

        var task = workOrder.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task is null)
            return Result.Failure("Tarea no encontrada.");

        var userId = _currentUser.UserId!.Value;
        var canManage = _currentUser.Permissions.Contains("canManageWorkOrders");
        var isAssigned = task.AssignedToId == userId || workOrder.AssignedMechanics.Any(m => m.UserId == userId);
        if (!canManage && !isAssigned)
            return Result.Failure("Solo el mecánico asignado o quien gestiona la OT puede cambiar el estado de la tarea.");

        try
        {
            task.UpdateStatus(request.Status, request.Observations, request.ActualHours);
            workOrder.RecalculateActualHours();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
