using MediatR;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.Inventory.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Inventory.Queries.GetAll;

public record GetInventoryQuery(
    int Page = 1,
    int PageSize = 10,
    InventoryCategory? Category = null,
    InventoryStatus? Status = null,
    string? Search = null,
    bool? LowStock = null,
    bool? ExpiringSoon = null
) : IRequest<PaginatedResult<InventoryItemDto>>;
