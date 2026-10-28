using MediatR;
using SIGMA.API.Reports.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.Reports.Queries.WorkOrders;

public record GetWorkOrdersReportQuery(
    DateTime DateFrom,
    DateTime DateTo,
    WorkOrderStatus? Status = null,
    WorkOrderType? Type = null,
    Guid? ClientId = null
) : IRequest<WorkOrdersReportDto>;
