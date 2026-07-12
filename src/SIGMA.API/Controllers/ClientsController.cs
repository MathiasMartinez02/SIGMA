using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Clients.Commands.Create;
using SIGMA.Application.Clients.Commands.Update;
using SIGMA.Application.Clients.Queries.GetAll;
using SIGMA.Application.Clients.Queries.GetById;
using SIGMA.Application.Common.Models;

namespace SIGMA.API.Controllers;

[ApiController]
[Route("api/v1/clients")]
[Authorize]
[Tags("Clients")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetClientsQuery(page, pageSize, search, isActive), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetClientByIdQuery(id), cancellationToken);
        if (result.Failed) return NotFound(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost]
    [Authorize(Policy = "CanManageClients")]
    public async Task<IActionResult> Create([FromBody] CreateClientCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<object>.Ok(result.Data));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageClients")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientCommand command, CancellationToken cancellationToken)
    {
        var cmd = command with { Id = id };
        var result = await _mediator.Send(cmd, cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }
}
