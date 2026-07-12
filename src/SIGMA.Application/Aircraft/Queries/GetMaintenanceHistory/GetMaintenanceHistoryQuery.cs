using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;

namespace SIGMA.Application.Aircraft.Queries.GetMaintenanceHistory;

public record GetMaintenanceHistoryQuery(Guid AircraftId) : IRequest<Result<IList<WorkOrderDto>>>;
