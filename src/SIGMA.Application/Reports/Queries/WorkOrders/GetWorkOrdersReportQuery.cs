using MediatR;
using SIGMA.Application.Reports.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Reports.Queries.WorkOrders;

public record GetWorkOrdersReportQuery(
    DateTime DateFrom,
    DateTime DateTo,
    WorkOrderStatus? Status = null,
    WorkOrderType? Type = null,
    Guid? ClientId = null
) : IRequest<WorkOrdersReportDto>;
