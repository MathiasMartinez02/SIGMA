using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Inventory.Commands.Update;

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
