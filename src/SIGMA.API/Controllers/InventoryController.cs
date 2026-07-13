using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inventory.Commands.AddMovement;
using SIGMA.Application.Inventory.Commands.ApproveMovement;
using SIGMA.Application.Inventory.Commands.Create;
using SIGMA.Application.Inventory.Commands.Delete;
using SIGMA.Application.Inventory.Commands.Update;
using SIGMA.Application.Inventory.Queries.GetAll;
using SIGMA.Application.Inventory.Queries.GetById;
using SIGMA.Application.Inventory.Queries.GetMovements;
using SIGMA.Domain.Enums;

namespace SIGMA.API.Controllers;

[ApiController]
[Route("api/v1/inventory")]
[Authorize]
[Tags("Inventory")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] InventoryCategory? category = null,
        [FromQuery] InventoryStatus? status = null,
        [FromQuery] string? search = null,
        [FromQuery] bool? lowStock = null,
        [FromQuery] bool? expiringSoon = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetInventoryQuery(page, pageSize, category, status, search, lowStock, expiringSoon), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetInventoryItemByIdQuery(id), cancellationToken);
        if (result.Failed) return NotFound(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpGet("{id:guid}/movements")]
    public async Task<IActionResult> GetMovements(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetInventoryMovementsQuery(id, page, pageSize), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost]
    [Authorize(Policy = "CanManageInventory")]
    public async Task<IActionResult> Create([FromBody] CreateInventoryItemCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<object>.Ok(result.Data));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageInventory")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInventoryItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateInventoryItemCommand(id, request.Description, request.Location, request.MinimumStock, request.UnitCost, request.CertificateNumber, request.ExpiryDate, request.MaximumStock),
            cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }

    [HttpPost("{id:guid}/movements")]
    [Authorize(Policy = "CanManageInventory")]
    public async Task<IActionResult> AddMovement(Guid id, [FromBody] AddMovementRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AddInventoryMovementCommand(id, request.Type, request.Quantity, request.WorkOrderId, request.Reason), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    // Aprueba un movimiento de salida de inventario, seteando el usuario actual como aprobador
    [HttpPatch("{itemId:guid}/movements/{movementId:guid}/approve")]
    [Authorize(Policy = "CanManageInventory")]
    public async Task<IActionResult> ApproveMovement(Guid itemId, Guid movementId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ApproveMovementCommand(itemId, movementId), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }

    // Da de baja (soft delete) un item de inventario, rechazando la baja si todavia tiene stock
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanManageInventory")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteInventoryItemCommand(id), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }
}

public record UpdateInventoryItemRequest(string Description, string Location, decimal MinimumStock, decimal UnitCost, string? CertificateNumber, DateTime? ExpiryDate, decimal? MaximumStock = null);
public record AddMovementRequest(MovementType Type, decimal Quantity, Guid? WorkOrderId, string Reason);
