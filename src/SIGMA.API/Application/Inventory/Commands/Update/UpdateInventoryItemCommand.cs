using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Inventory.Commands.Update;

public record UpdateInventoryItemCommand(
    Guid Id,
    string Description,
    string Location,
    decimal MinimumStock,
    decimal UnitCost,
    string? CertificateNumber,
    DateTime? ExpiryDate,
    decimal? MaximumStock = null
) : IRequest<Result>;
