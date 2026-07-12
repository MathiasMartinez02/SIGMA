using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inventory.DTOs;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Inventory.Commands.Create;

public class CreateInventoryItemCommandHandler : IRequestHandler<CreateInventoryItemCommand, Result<InventoryItemDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateInventoryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<InventoryItemDto>> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var partNumberUpper = request.PartNumber.Trim().ToUpperInvariant();

        var exists = await _context.InventoryItems
            .AnyAsync(i => i.PartNumber == partNumberUpper && !i.IsDeleted, cancellationToken);

        if (exists)
            return Result<InventoryItemDto>.Failure($"Ya existe un ítem con el número de parte '{request.PartNumber}'.");

        var item = InventoryItem.Create(
            request.PartNumber,
            request.Description,
            request.Category,
            request.Manufacturer,
            request.Location,
            request.MinimumStock,
            request.Unit,
            request.UnitCost,
            request.CertificationRequired,
            request.ExpiryDate,
            request.AltPartNumbers);

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);

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
