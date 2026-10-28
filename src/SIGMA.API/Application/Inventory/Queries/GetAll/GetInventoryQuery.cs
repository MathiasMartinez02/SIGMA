using MediatR;
using SIGMA.API.Common.Pagination;
using SIGMA.API.Inventory.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.Inventory.Queries.GetAll;

public record GetInventoryQuery(
    int Page = 1,
    int PageSize = 10,
    InventoryCategory? Category = null,
    InventoryStatus? Status = null,
    string? Search = null,
    bool? LowStock = null,
    bool? ExpiringSoon = null
) : IRequest<PaginatedResult<InventoryItemDto>>;
