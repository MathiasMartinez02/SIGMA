using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.WorkOrders.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateTaskStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.WorkOrderTasks
            .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.WorkOrderId == request.WorkOrderId, cancellationToken);

        if (task is null)
            return Result.Failure("Tarea no encontrada.");

        task.UpdateStatus(request.Status, request.Observations, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
