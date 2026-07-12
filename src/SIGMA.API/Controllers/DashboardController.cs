using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Dashboard.Queries.GetDashboard;

namespace SIGMA.API.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize(Policy = "CanViewDashboard")]
[Tags("Dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDashboardQuery(), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }
}
