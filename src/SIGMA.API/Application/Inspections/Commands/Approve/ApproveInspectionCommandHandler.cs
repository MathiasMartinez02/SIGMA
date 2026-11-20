using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Exceptions;

namespace SIGMA.API.Inspections.Commands.Approve;

public class ApproveInspectionCommandHandler : IRequestHandler<ApproveInspectionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ApproveInspectionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    // Aprobacion de la inspeccion y firma de las tareas que la requerian
    public async Task<Result> Handle(ApproveInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .Include(i => i.ChecklistSections).ThenInclude(s => s.Items)
            .Include(i => i.WorkOrder).ThenInclude(w => w.Tasks)
            .AsSplitQuery()
            .FirstOrDefaultAsync(i => i.Id == request.InspectionId && !i.IsDeleted, cancellationToken);

        if (inspection is null)
            return Result.Failure("La inspección no fue encontrada.");

        var userId = _currentUser.UserId!.Value;

        try
        {
            inspection.Approve(userId, request.Observations);
            inspection.WorkOrder.MarkInspectionTasksInspected(userId);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
