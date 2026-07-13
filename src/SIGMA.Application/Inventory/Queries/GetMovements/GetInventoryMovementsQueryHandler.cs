using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.Inventory.DTOs;

namespace SIGMA.Application.Inventory.Queries.GetMovements;

public class GetInventoryMovementsQueryHandler : IRequestHandler<GetInventoryMovementsQuery, PaginatedResult<InventoryMovementDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryMovementsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<InventoryMovementDto>> Handle(GetInventoryMovementsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.InventoryMovements
            .Include(m => m.PerformedBy)
            .Where(m => m.ItemId == request.ItemId)
            .OrderByDescending(m => m.PerformedAt)
            .AsQueryable();

        var mapped = query.Select(m => new InventoryMovementDto
        {
            Id = m.Id,
            Type = m.Type,
            Quantity = m.Quantity,
            PreviousStock = m.PreviousStock,
            NewStock = m.NewStock,
            WorkOrderId = m.WorkOrderId,
            Reason = m.Reason,
            PerformedById = m.PerformedById,
            PerformedByName = m.PerformedBy.FirstName + " " + m.PerformedBy.LastName,
            PerformedAt = m.PerformedAt,
            // Se agrega el nombre de quien aprobo el movimiento (antes el DTO no lo exponia y el frontend
            // nunca podia saber si un movimiento ya estaba aprobado). No hay navegacion ApprovedBy en la entidad,
            // se resuelve el nombre con una subconsulta correlacionada contra Users.
            ApprovedById = m.ApprovedById,
            ApprovedByName = m.ApprovedById.HasValue
                ? _context.Users.Where(u => u.Id == m.ApprovedById).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault()
                : null
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
