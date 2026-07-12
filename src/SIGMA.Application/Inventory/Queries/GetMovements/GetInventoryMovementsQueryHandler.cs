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
            PerformedAt = m.PerformedAt
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
