using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inventory.DTOs;

namespace SIGMA.Application.Inventory.Queries.GetById;

public class GetInventoryItemByIdQueryHandler : IRequestHandler<GetInventoryItemByIdQuery, Result<InventoryItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryItemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<InventoryItemDto>> Handle(GetInventoryItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

        if (item is null)
            return Result<InventoryItemDto>.Failure("El ítem de inventario no fue encontrado.");

        var dto = new InventoryItemDto
        {
            Id = item.Id,
            PartNumber = item.PartNumber,
            Description = item.Description,
            Category = item.Category,
            Manufacturer = item.Manufacturer,
            Location = item.Location,
            CurrentStock = item.CurrentStock,
            MinimumStock = item.MinimumStock,
            Unit = item.Unit,
            UnitCost = item.UnitCost,
            Status = item.Status,
            ExpiryDate = item.ExpiryDate,
            CertificationRequired = item.CertificationRequired,
            CertificateNumber = item.CertificateNumber,
            LastMovementDate = item.LastMovementDate,
            CreatedAt = item.CreatedAt
        };

        return Result<InventoryItemDto>.Success(dto);
    }
}
