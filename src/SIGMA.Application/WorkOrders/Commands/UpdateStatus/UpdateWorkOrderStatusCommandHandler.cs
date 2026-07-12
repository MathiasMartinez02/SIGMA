using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Entities;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Application.WorkOrders.Commands.UpdateStatus;

public class UpdateWorkOrderStatusCommandHandler : IRequestHandler<UpdateWorkOrderStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateWorkOrderStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateWorkOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _context.WorkOrders
            .Include(w => w.Tasks)
            .Include(w => w.Timeline)
            .FirstOrDefaultAsync(w => w.Id == request.WorkOrderId, cancellationToken);

        if (workOrder is null)
            return Result.Failure("Orden de trabajo no encontrada.");

        try
        {
            var userId = _currentUser.UserId!.Value;
            var userName = _currentUser.FullName ?? "Sistema";
            var userRole = _currentUser.Role ?? "sistema";

            workOrder.TransitionTo(request.NewStatus, userId, userName, userRole);

            if (request.NewStatus == WorkOrderStatus.EnInspeccion)
            {
                var aircraft = await _context.Aircraft
                    .FirstAsync(a => a.Id == workOrder.AircraftId, cancellationToken);

                var inspection = Inspection.Create(
                    workOrder.Id,
                    workOrder.Type.ToString(),
                    workOrder.AircraftId,
                    aircraft.TotalFlightHours,
                    DateTime.UtcNow);

                _context.Inspections.Add(inspection);
                aircraft.UpdateStatus(AircraftStatus.EnInspeccion);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidStatusTransitionException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
