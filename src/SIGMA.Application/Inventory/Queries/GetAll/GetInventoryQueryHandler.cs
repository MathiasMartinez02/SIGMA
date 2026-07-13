using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.Inventory.DTOs;

namespace SIGMA.Application.Inventory.Queries.GetAll;

public class GetInventoryQueryHandler : IRequestHandler<GetInventoryQuery, PaginatedResult<InventoryItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<InventoryItemDto>> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.InventoryItems
            .Where(i => !i.IsDeleted)
            .AsQueryable();

        if (request.Category.HasValue)
            query = query.Where(i => i.Category == request.Category.Value);

        if (request.Status.HasValue)
            query = query.Where(i => i.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(i =>
                i.PartNumber.ToLower().Contains(search) ||
                i.Description.ToLower().Contains(search));
        }

        if (request.LowStock == true)
            query = query.Where(i => i.CurrentStock <= i.MinimumStock);

        if (request.ExpiringSoon == true)
        {
            var expiryThreshold = DateTime.UtcNow.AddDays(90);
            query = query.Where(i => i.ExpiryDate.HasValue && i.ExpiryDate.Value <= expiryThreshold && i.ExpiryDate.Value >= DateTime.UtcNow);
        }

        query = query.OrderBy(i => i.PartNumber);

        var mapped = query.Select(i => new InventoryItemDto
        {
            Id = i.Id,
            PartNumber = i.PartNumber,
            Description = i.Description,
            Category = i.Category,
            Manufacturer = i.Manufacturer,
            Location = i.Location,
            CurrentStock = i.CurrentStock,
            MinimumStock = i.MinimumStock,
            MaximumStock = i.MaximumStock,
            Unit = i.Unit,
            UnitCost = i.UnitCost,
            Status = i.Status,
            ExpiryDate = i.ExpiryDate,
            CertificationRequired = i.CertificationRequired,
            CertificateNumber = i.CertificateNumber,
            LastMovementDate = i.LastMovementDate,
            CreatedAt = i.CreatedAt
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
