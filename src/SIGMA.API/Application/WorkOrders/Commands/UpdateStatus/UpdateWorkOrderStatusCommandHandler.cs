using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using SIGMA.API.Exceptions;
using SIGMA.API.WorkOrders.Services;

namespace SIGMA.API.WorkOrders.Commands.UpdateStatus;

public class UpdateWorkOrderStatusCommandHandler : IRequestHandler<UpdateWorkOrderStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateWorkOrderStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    // Transicion de la OT con apertura de inspeccion y estado de la aeronave
    public async Task<Result> Handle(UpdateWorkOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _context.WorkOrders
            .Include(w => w.Tasks)
            .Include(w => w.Timeline)
            .Include(w => w.Inspections)
            .AsSplitQuery()
            .FirstOrDefaultAsync(w => w.Id == request.WorkOrderId && !w.IsDeleted, cancellationToken);

        if (workOrder is null)
            return Result.Failure("Orden de trabajo no encontrada.");

        var aircraft = await _context.Aircraft.FirstAsync(a => a.Id == workOrder.AircraftId, cancellationToken);

        try
        {
            workOrder.TransitionTo(request.NewStatus, _currentUser.UserId!.Value,
                _currentUser.FullName ?? "Sistema", _currentUser.Role ?? "sistema");

            if (request.NewStatus == WorkOrderStatus.EnInspeccion)
            {
                var (inspection, isNew) = workOrder.OpenInspection(aircraft.TotalFlightHours);
                if (isNew) _context.Inspections.Add(inspection);
            }

            if (request.NewStatus == WorkOrderStatus.Finalizada)
                aircraft.RegisterCompletedInspection(workOrder.Type, DateTime.UtcNow);

            await AircraftStatusSync.ApplyAsync(_context, workOrder, aircraft, cancellationToken);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
