using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Inventory.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.Inventory.Commands.Create;

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
