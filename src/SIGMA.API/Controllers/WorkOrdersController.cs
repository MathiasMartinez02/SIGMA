using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.Commands.AddDocument;
using SIGMA.Application.WorkOrders.Commands.AddMechanic;
using SIGMA.Application.WorkOrders.Commands.AddTask;
using SIGMA.Application.WorkOrders.Commands.Create;
using SIGMA.Application.WorkOrders.Commands.RemoveMechanic;
using SIGMA.Application.WorkOrders.Commands.Update;
using SIGMA.Application.WorkOrders.Commands.UpdateStatus;
using SIGMA.Application.WorkOrders.Commands.UpdateTaskStatus;
using SIGMA.Application.WorkOrders.Queries.GetAll;
using SIGMA.Application.WorkOrders.Queries.GetById;
using SIGMA.Domain.Enums;

namespace SIGMA.API.Controllers;

[ApiController]
[Route("api/v1/work-orders")]
[Authorize]
[Tags("WorkOrders")]
public class WorkOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkOrdersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] WorkOrderStatus? status = null,
        [FromQuery] WorkOrderPriority? priority = null,
        [FromQuery] WorkOrderType? type = null,
        [FromQuery] Guid? aircraftId = null,
        [FromQuery] Guid? clientId = null,
        [FromQuery] Guid? mechanicId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetWorkOrdersQuery(page, pageSize, status, priority, type, aircraftId, clientId, mechanicId, dateFrom, dateTo, search),
            cancellationToken);

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWorkOrderByIdQuery(id), cancellationToken);
        if (result.Failed) return NotFound(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost]
    [Authorize(Policy = "CanManageWorkOrders")]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<object>.Ok(result.Data));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageWorkOrders")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateWorkOrderCommand(id, request.Description, request.EstimatedHours, request.IntakeDate, request.EstimatedEndDate, request.Priority),
            cancellationToken);

        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok("Orden de trabajo actualizada."));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "CanManageWorkOrders")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateWorkOrderStatusCommand(id, request.Status), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok("Estado actualizado correctamente."));
    }

    [HttpPost("{id:guid}/tasks")]
    [Authorize(Policy = "CanManageWorkOrders")]
    public async Task<IActionResult> AddTask(Guid id, [FromBody] AddTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddWorkOrderTaskCommand(id, request.Title, request.Description, request.EstimatedHours, request.RequiresInspection),
            cancellationToken);

        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPatch("{id:guid}/tasks/{taskId:guid}/status")]
    public async Task<IActionResult> UpdateTaskStatus(Guid id, Guid taskId, [FromBody] UpdateTaskStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateTaskStatusCommand(id, taskId, request.Status, request.Observations), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }

    // Endpoint que faltaba (Fase 4): WorkOrderDocument existia en el Domain pero no tenia Command/Controller
    [HttpPost("{id:guid}/documents")]
    [Authorize(Policy = "CanManageWorkOrders")]
    public async Task<IActionResult> AddDocument(Guid id, [FromBody] AddWorkOrderDocumentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AddWorkOrderDocumentCommand(id, request.Name, request.Type, request.FileUrl), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost("{id:guid}/mechanics")]
    [Authorize(Policy = "CanManageWorkOrders")]
    public async Task<IActionResult> AddMechanic(Guid id, [FromBody] AddMechanicRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AddMechanicCommand(id, request.UserId), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok("Mecánico asignado correctamente."));
    }

    [HttpDelete("{id:guid}/mechanics/{userId:guid}")]
    [Authorize(Policy = "CanManageWorkOrders")]
    public async Task<IActionResult> RemoveMechanic(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemoveMechanicCommand(id, userId), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok("Mecánico removido correctamente."));
    }
}

// MODIFICADO: agrega IntakeDate para permitir editar la fecha de ingreso de la OT desde el endpoint
// ANTERIOR: UpdateWorkOrderRequest(string Description, decimal EstimatedHours, DateTime EstimatedEndDate, WorkOrderPriority Priority) sin fecha de ingreso
public record UpdateWorkOrderRequest(string Description, decimal EstimatedHours, DateTime IntakeDate, DateTime EstimatedEndDate, WorkOrderPriority Priority);
public record UpdateStatusRequest(WorkOrderStatus Status);
public record AddTaskRequest(string Title, string Description, decimal EstimatedHours, bool RequiresInspection);
public record UpdateTaskStatusRequest(WorkOrderTaskStatus Status, string? Observations);
public record AddMechanicRequest(Guid UserId);
public record AddWorkOrderDocumentRequest(string Name, string Type, string FileUrl);
