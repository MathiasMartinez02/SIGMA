using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Budgets.Commands.Create;
using SIGMA.Application.Budgets.Commands.UpdateStatus;
using SIGMA.Application.Budgets.Queries.GetAll;
using SIGMA.Application.Budgets.Queries.GetById;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.API.Controllers;

// Endpoints de gestion de presupuestos (Fase 6, MVP de Administrativo): alta, consulta y aceptacion/rechazo.
// Reusa la policy "CanManageClients" para las mutaciones -- un presupuesto es, en esencia, un documento
// comercial atado al cliente, y esos 3 roles (Gerente/OficinaTecnica/Administracion) ya son quienes gestionan
// clientes hoy; no se creo una policy nueva para no invadir el trabajo en paralelo de la Fase 7 (roles/permisos).
[ApiController]
[Route("api/v1/budgets")]
[Authorize]
[Tags("Budgets")]
public class BudgetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BudgetsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] BudgetStatus? status = null,
        [FromQuery] Guid? clientId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetBudgetsQuery(page, pageSize, status, clientId), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBudgetByIdQuery(id), cancellationToken);
        if (result.Failed) return NotFound(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost]
    [Authorize(Policy = "CanManageClients")]
    public async Task<IActionResult> Create([FromBody] CreateBudgetCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<object>.Ok(result.Data));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "CanManageClients")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBudgetStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateBudgetStatusCommand(id, request.Status), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok("Estado del presupuesto actualizado correctamente."));
    }
}

// Body del endpoint de cambio de estado de presupuesto (solo Aceptado o Rechazado)
public record UpdateBudgetStatusRequest(BudgetStatus Status);
