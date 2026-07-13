using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Aircraft.Queries.GetInventoryUsage;

// Resuelve los movimientos de salida de inventario vinculados, via WorkOrderId, a OTs de la aeronave solicitada
public class GetAircraftInventoryUsageQueryHandler : IRequestHandler<GetAircraftInventoryUsageQuery, Result<IList<AircraftInventoryUsageDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAircraftInventoryUsageQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IList<AircraftInventoryUsageDto>>> Handle(GetAircraftInventoryUsageQuery request, CancellationToken cancellationToken)
    {
        var aircraftExists = await _context.Aircraft
            .AnyAsync(a => a.Id == request.AircraftId && !a.IsDeleted, cancellationToken);

        if (!aircraftExists)
            return Result<IList<AircraftInventoryUsageDto>>.Failure("La aeronave no fue encontrada.");

        // Join real: InventoryMovement.WorkOrderId -> WorkOrder.Id -> WorkOrder.AircraftId == request.AircraftId
        var dtos = await _context.InventoryMovements
            .Include(m => m.Item)
            .Include(m => m.WorkOrder)
            .Where(m => m.Type == MovementType.Salida
                && m.WorkOrder != null
                && m.WorkOrder.AircraftId == request.AircraftId)
            .OrderByDescending(m => m.PerformedAt)
            .Select(m => new AircraftInventoryUsageDto
            {
                PartNumber = m.Item.PartNumber,
                ItemDescription = m.Item.Description,
                Quantity = m.Quantity,
                MovementDate = m.PerformedAt,
                WorkOrderId = m.WorkOrder!.Id,
                WorkOrderNumber = m.WorkOrder!.Number
            })
            .ToListAsync(cancellationToken);

        return Result<IList<AircraftInventoryUsageDto>>.Success(dtos);
    }
}
