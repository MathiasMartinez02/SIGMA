using MediatR;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.Inventory.DTOs;

namespace SIGMA.Application.Inventory.Queries.GetMovements;

public record GetInventoryMovementsQuery(
    Guid ItemId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedResult<InventoryMovementDto>>;
