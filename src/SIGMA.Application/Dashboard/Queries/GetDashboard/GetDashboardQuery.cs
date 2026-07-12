using MediatR;
using SIGMA.Application.Dashboard.DTOs;

namespace SIGMA.Application.Dashboard.Queries.GetDashboard;

public record GetDashboardQuery() : IRequest<DashboardDto>;
