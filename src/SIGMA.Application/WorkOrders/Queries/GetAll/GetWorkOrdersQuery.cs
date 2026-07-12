using MediatR;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.WorkOrders.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.WorkOrders.Queries.GetAll;

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
