using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;

namespace SIGMA.API.Aircraft.Queries.GetMaintenanceHistory;

public record GetMaintenanceHistoryQuery(Guid AircraftId) : IRequest<Result<IList<WorkOrderDto>>>;
