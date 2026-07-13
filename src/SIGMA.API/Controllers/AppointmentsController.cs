using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Appointments.Commands.ConvertToWorkOrder;
using SIGMA.Application.Appointments.Commands.Create;
using SIGMA.Application.Appointments.Commands.UpdateStatus;
using SIGMA.Application.Appointments.Queries.GetAll;
using SIGMA.Application.Appointments.Queries.GetById;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.API.Controllers;

// Endpoints de gestion de turnos: alta, consulta, confirmacion/cancelacion y conversion a orden de trabajo
[ApiController]
[Route("api/v1/appointments")]
[Authorize]
[Tags("Appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] AppointmentStatus? status = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAppointmentsQuery(page, pageSize, status, dateFrom, dateTo),
            cancellationToken);

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAppointmentByIdQuery(id), cancellationToken);
        if (result.Failed) return NotFound(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost]
    [Authorize(Policy = "CanManageWorkOrders")]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<object>.Ok(result.Data));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "CanManageWorkOrders")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateAppointmentStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateAppointmentStatusCommand(id, request.Status), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok("Estado del turno actualizado correctamente."));
    }

    [HttpPost("{id:guid}/convert-to-work-order")]
    [Authorize(Policy = "CanManageWorkOrders")]
    public async Task<IActionResult> ConvertToWorkOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ConvertAppointmentToWorkOrderCommand(id), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }
}

// Body del endpoint de cambio de estado de turno (solo Confirmado o Cancelado)
public record UpdateAppointmentStatusRequest(AppointmentStatus Status);
