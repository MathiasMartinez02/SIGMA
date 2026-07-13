using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Aircraft.Commands.AddComponent;
using SIGMA.Application.Aircraft.Commands.AddDocument;
using SIGMA.Application.Aircraft.Commands.AddFlightRecord;
using SIGMA.Application.Aircraft.Commands.Create;
using SIGMA.Application.Aircraft.Commands.Update;
using SIGMA.Application.Aircraft.Commands.UpdateStatus;
using SIGMA.Application.Aircraft.Queries.GetAll;
using SIGMA.Application.Aircraft.Queries.GetById;
using SIGMA.Application.Aircraft.Queries.GetInventoryUsage;
using SIGMA.Application.Aircraft.Queries.GetMaintenanceHistory;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.API.Controllers;

[ApiController]
[Route("api/v1/aircraft")]
[Authorize]
[Tags("Aircraft")]
public class AircraftController : ControllerBase
{
    private readonly IMediator _mediator;

    public AircraftController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] AircraftStatus? status = null,
        [FromQuery] AircraftCategory? category = null,
        [FromQuery] Guid? clientId = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAircraftQuery(page, pageSize, status, category, clientId, search), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAircraftByIdQuery(id), cancellationToken);
        if (result.Failed) return NotFound(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost]
    [Authorize(Policy = "CanManageAircraft")]
    public async Task<IActionResult> Create([FromBody] CreateAircraftCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<object>.Ok(result.Data));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageAircraft")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAircraftCommand command, CancellationToken cancellationToken)
    {
        var cmd = command with { Id = id };
        var result = await _mediator.Send(cmd, cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "CanManageAircraft")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateAircraftStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAircraftStatusCommand(id, request.Status), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }

    [HttpPost("{id:guid}/flight-records")]
    public async Task<IActionResult> AddFlightRecord(Guid id, [FromBody] AddFlightRecordRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddFlightRecordCommand(id, request.Date, request.Duration, request.Landings, request.Pilot, request.Origin, request.Destination, request.Notes),
            cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost("{id:guid}/documents")]
    public async Task<IActionResult> AddDocument(Guid id, [FromBody] AddAircraftDocumentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddAircraftDocumentCommand(id, request.Type, request.Name, request.FileUrl, request.ExpiryDate),
            cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost("{id:guid}/components")]
    public async Task<IActionResult> AddComponent(Guid id, [FromBody] AddAircraftComponentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddAircraftComponentCommand(id, request.Name, request.PartNumber, request.SerialNumber,
                request.Manufacturer, request.InstallDate, request.InstallHours, request.LifeLimitHours, request.OverhaulDueHours),
            cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpGet("{id:guid}/maintenance-history")]
    public async Task<IActionResult> GetMaintenanceHistory(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMaintenanceHistoryQuery(id), cancellationToken);
        if (result.Failed) return NotFound(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    // Devuelve la trazabilidad de repuestos usados en OTs de la aeronave (numero de parte, descripcion, cantidad, fecha y OT)
    [HttpGet("{id:guid}/inventory-usage")]
    public async Task<IActionResult> GetInventoryUsage(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAircraftInventoryUsageQuery(id), cancellationToken);
        if (result.Failed) return NotFound(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }
}

public record UpdateAircraftStatusRequest(AircraftStatus Status);
public record AddFlightRecordRequest(DateTime Date, decimal Duration, int Landings, string Pilot, string Origin, string Destination, string? Notes);
public record AddAircraftDocumentRequest(string Type, string Name, string FileUrl, DateTime? ExpiryDate);
public record AddAircraftComponentRequest(string Name, string PartNumber, string SerialNumber, string Manufacturer, DateTime InstallDate, decimal InstallHours, decimal? LifeLimitHours, decimal? OverhaulDueHours);
