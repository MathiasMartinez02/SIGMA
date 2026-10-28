using MediatR;
using SIGMA.API.Dashboard.DTOs;

namespace SIGMA.API.Dashboard.Queries.GetDashboard;

public record GetDashboardQuery() : IRequest<DashboardDto>;
