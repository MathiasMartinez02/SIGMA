using MediatR;
using SIGMA.API.Common.Pagination;
using SIGMA.API.WorkOrders.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.WorkOrders.Queries.GetAll;

public record GetWorkOrdersQuery(
    int Page = 1,
    int PageSize = 10,
    WorkOrderStatus? Status = null,
    WorkOrderPriority? Priority = null,
    WorkOrderType? Type = null,
    Guid? AircraftId = null,
    Guid? ClientId = null,
    Guid? MechanicId = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    string? Search = null
) : IRequest<PaginatedResult<WorkOrderDto>>;
