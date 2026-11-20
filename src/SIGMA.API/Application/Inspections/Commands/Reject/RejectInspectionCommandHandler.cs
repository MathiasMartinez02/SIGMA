using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;
using SIGMA.API.Exceptions;
using SIGMA.API.WorkOrders.Services;

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

    // Rechazo de la inspeccion: la OT vuelve a En proceso para corregir
    public async Task<Result> Handle(RejectInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .Include(i => i.WorkOrder).ThenInclude(w => w.Timeline)
            .Include(i => i.WorkOrder).ThenInclude(w => w.Inspections)
            .Include(i => i.Aircraft)
            .AsSplitQuery()
            .FirstOrDefaultAsync(i => i.Id == request.InspectionId && !i.IsDeleted, cancellationToken);

        if (inspection is null)
            return Result.Failure("La inspección no fue encontrada.");

        var userId = _currentUser.UserId!.Value;

        try
        {
            inspection.Reject(request.RejectionReason, userId);

            if (inspection.WorkOrder.Status == WorkOrderStatus.EnInspeccion)
            {
                inspection.WorkOrder.TransitionTo(WorkOrderStatus.EnProceso, userId,
                    _currentUser.FullName ?? "Sistema", _currentUser.Role ?? "sistema");
                await AircraftStatusSync.ApplyAsync(_context, inspection.WorkOrder, inspection.Aircraft, cancellationToken);
            }
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
