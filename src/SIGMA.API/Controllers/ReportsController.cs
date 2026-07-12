using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Reports.Queries.AircraftStatus;
using SIGMA.Application.Reports.Queries.Inspections;
using SIGMA.Application.Reports.Queries.Inventory;
using SIGMA.Application.Reports.Queries.WorkOrders;
using SIGMA.Domain.Enums;

namespace SIGMA.API.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize(Policy = "CanViewReports")]
[Tags("Reports")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("work-orders")]
    public async Task<IActionResult> GetWorkOrdersReport(
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] WorkOrderStatus? status = null,
        [FromQuery] WorkOrderType? type = null,
        [FromQuery] Guid? clientId = null,
        CancellationToken cancellationToken = default)
    {
        var from = dateFrom ?? DateTime.UtcNow.AddMonths(-3);
        var to = dateTo ?? DateTime.UtcNow;
        var result = await _mediator.Send(new GetWorkOrdersReportQuery(from, to, status, type, clientId), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("inspections")]
    public async Task<IActionResult> GetInspectionsReport(
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] string? type = null,
        [FromQuery] Guid? inspectorId = null,
        CancellationToken cancellationToken = default)
    {
        var from = dateFrom ?? DateTime.UtcNow.AddMonths(-3);
        var to = dateTo ?? DateTime.UtcNow;
        var result = await _mediator.Send(new GetInspectionsReportQuery(from, to, type, inspectorId), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("aircraft-status")]
    public async Task<IActionResult> GetAircraftStatusReport(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAircraftStatusReportQuery(), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("inventory")]
    public async Task<IActionResult> GetInventoryReport(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInventoryReportQuery(), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }
}
