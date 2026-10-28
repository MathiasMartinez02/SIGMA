using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.Inspections.Commands.Reject;

public class RejectInspectionCommandHandler : IRequestHandler<RejectInspectionCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RejectInspectionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(RejectInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .Include(i => i.WorkOrder)
            .FirstOrDefaultAsync(i => i.Id == request.InspectionId && !i.IsDeleted, cancellationToken);

        if (inspection is null)
            return Result.Failure("La inspección no fue encontrada.");

        var userId = _currentUser.UserId!.Value;

        try
        {
            inspection.Reject(request.RejectionReason, userId);

            if (inspection.WorkOrder.Status == WorkOrderStatus.EnInspeccion)
            {
                inspection.WorkOrder.TransitionTo(
                    WorkOrderStatus.EnProceso,
                    userId,
                    _currentUser.FullName ?? "Sistema",
                    _currentUser.Role ?? "sistema");
            }
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
