using MediatR;
using SIGMA.API.Common.Pagination;
using SIGMA.API.Inventory.DTOs;

namespace SIGMA.API.Inventory.Queries.GetMovements;

public record GetInventoryMovementsQuery(
    Guid ItemId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedResult<InventoryMovementDto>>;
