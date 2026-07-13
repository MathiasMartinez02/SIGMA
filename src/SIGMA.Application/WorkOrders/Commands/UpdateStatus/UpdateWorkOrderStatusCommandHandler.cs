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
        // MODIFICADO: se agrega AsSplitQuery(). Con dos Include de colecciones hermanas (Tasks y Timeline) en una sola
        // consulta, EF Core genera un JOIN cartesiano sin ORDER BY completo (advertencia MultipleCollectionIncludeWarning)
        // que corrompe el tracking de la entidad Timeline recien agregada: el INSERT del nuevo registro de timeline
        // se emite como UPDATE de una fila que no existe todavia, y SaveChangesAsync tira DbUpdateConcurrencyException
        // en CUALQUIER cambio de estado de la OT. AsSplitQuery separa cada colección en su propia consulta y evita el problema.
        // ANTERIOR: .Include(w => w.Tasks).Include(w => w.Timeline) sin AsSplitQuery()
        var workOrder = await _context.WorkOrders
            .Include(w => w.Tasks)
            .Include(w => w.Timeline)
            .AsSplitQuery()
            .FirstOrDefaultAsync(w => w.Id == request.WorkOrderId, cancellationToken);

        if (workOrder is null)
            return Result.Failure("Orden de trabajo no encontrada.");

        try
        {
            var userId = _currentUser.UserId!.Value;
            var userName = _currentUser.FullName ?? "Sistema";
            var userRole = _currentUser.Role ?? "sistema";

            workOrder.TransitionTo(request.NewStatus, userId, userName, userRole);

            // MODIFICADO: se agrega explicitamente al DbSet la entrada de timeline recien creada por TransitionTo.
            // Por alguna razon el tracker de EF no la detecta como "Added" solo por estar en la coleccion de navegacion
            // del WorkOrder ya trackeado (la trata como "Modified" e intenta un UPDATE sobre una fila que no existe,
            // lo que tira DbUpdateConcurrencyException). Agregarla explicitamente al DbSet fuerza el estado correcto,
            // igual que ya se hace mas abajo con "_context.Inspections.Add(inspection)".
            // ANTERIOR: se confiaba unicamente en que Timeline.Add(...) dentro de TransitionTo la trackeara bien
            _context.WorkOrderTimelines.Add(workOrder.Timeline.Last());

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
