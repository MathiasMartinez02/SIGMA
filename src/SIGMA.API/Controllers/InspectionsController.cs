using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inspections.Commands.Approve;
using SIGMA.Application.Inspections.Commands.Reject;
using SIGMA.Application.Inspections.Commands.UpdateChecklistItem;
using SIGMA.Application.Inspections.Queries.GetAll;
using SIGMA.Application.Inspections.Queries.GetById;
using SIGMA.Domain.Enums;

namespace SIGMA.API.Controllers;

[ApiController]
[Route("api/v1/inspections")]
[Authorize]
[Tags("Inspections")]
public class InspectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InspectionsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] InspectionStatus? status = null,
        [FromQuery] string? type = null,
        [FromQuery] Guid? inspectorId = null,
        [FromQuery] Guid? aircraftId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetInspectionsQuery(page, pageSize, status, type, inspectorId, aircraftId, dateFrom, dateTo, search),
            cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInspectionByIdQuery(id), cancellationToken);
        if (result.Failed) return NotFound(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = "CanApproveInspections")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ApproveInspectionCommand(id, request.Observations), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok("Inspección aprobada."));
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = "CanApproveInspections")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RejectInspectionCommand(id, request.RejectionReason), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok("Inspección rechazada."));
    }

    [HttpPatch("{id:guid}/checklist/{itemId:guid}")]
    public async Task<IActionResult> UpdateChecklistItem(
        Guid id, Guid itemId,
        [FromBody] UpdateChecklistItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateChecklistItemCommand(id, itemId, request.Status, request.Observations), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }
}

public record ApproveRequest(string? Observations);
public record RejectRequest(string RejectionReason);
public record UpdateChecklistItemRequest(ChecklistItemStatus Status, string? Observations);
