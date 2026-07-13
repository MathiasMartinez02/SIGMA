using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Inventory.DTOs;
using SIGMA.Application.Reports.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Reports.Queries.Inventory;

public class GetInventoryReportQueryHandler : IRequestHandler<GetInventoryReportQuery, InventoryReportDto>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryReportDto> Handle(GetInventoryReportQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.InventoryItems
            .Where(i => !i.IsDeleted)
            .OrderBy(i => i.PartNumber)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        var dtos = items.Select(i => new InventoryItemDto
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
        }).ToList();

        var lowStockCount = items.Count(i => i.CurrentStock <= i.MinimumStock && i.CurrentStock > 0);
        var outOfStockCount = items.Count(i => i.Status == InventoryStatus.SinStock);
        var expiredCount = items.Count(i => i.ExpiryDate.HasValue && i.ExpiryDate.Value.Date < now.Date);
        var totalValue = items.Sum(i => i.CurrentStock * i.UnitCost);

        return new InventoryReportDto
        {
            Items = dtos,
            LowStockCount = lowStockCount,
            OutOfStockCount = outOfStockCount,
            ExpiredCount = expiredCount,
            TotalValue = Math.Round(totalValue, 2)
        };
    }
}
