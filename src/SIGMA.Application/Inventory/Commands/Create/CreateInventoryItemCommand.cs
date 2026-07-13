using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inventory.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Inventory.Commands.Create;

public record CreateInventoryItemCommand(
    string PartNumber,
    string Description,
    InventoryCategory Category,
    string Manufacturer,
    string Location,
    decimal MinimumStock,
    string Unit,
    decimal UnitCost,
    bool CertificationRequired = false,
    DateTime? ExpiryDate = null,
    string? AltPartNumbers = null,
    decimal? MaximumStock = null
) : IRequest<Result<InventoryItemDto>>;
