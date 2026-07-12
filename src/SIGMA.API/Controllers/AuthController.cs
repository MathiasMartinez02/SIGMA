using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Auth.Commands.Login;
using SIGMA.Application.Auth.Commands.Logout;
using SIGMA.Application.Auth.Commands.Refresh;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Tags("Auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public AuthController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password), cancellationToken);

        if (result.Failed)
            return Unauthorized(ApiResponse<object>.Fail(result.Errors));

        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken), cancellationToken);

        if (result.Failed)
            return Unauthorized(ApiResponse<object>.Fail(result.Errors));

        return Ok(ApiResponse<object>.Ok(new { accessToken = result.Data }));
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;
        await _mediator.Send(new LogoutCommand(userId), cancellationToken);
        return NoContent();
    }
}

public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
