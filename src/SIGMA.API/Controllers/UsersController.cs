using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Users.Commands.Create;
using SIGMA.Application.Users.Commands.Update;
using SIGMA.Application.Users.Commands.UpdateStatus;
using SIGMA.Application.Users.Queries.GetAll;
using SIGMA.Application.Users.Queries.GetById;
using SIGMA.Domain.Enums;

namespace SIGMA.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize(Policy = "CanManageUsers")]
[Tags("Users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] UserRole? role = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUsersQuery(page, pageSize, role, isActive, search), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        if (result.Failed) return NotFound(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<object>.Ok(result.Data));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var cmd = command with { Id = id };
        var result = await _mediator.Send(cmd, cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUserStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateUserStatusCommand(id, request.IsActive), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }
}

public record UpdateUserStatusRequest(bool IsActive);
