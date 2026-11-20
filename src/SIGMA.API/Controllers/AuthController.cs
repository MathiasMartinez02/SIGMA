using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.API.Auth.Commands.ForgotPassword;
using SIGMA.API.Auth.Commands.Login;
using SIGMA.API.Auth.Commands.Logout;
using SIGMA.API.Auth.Commands.Refresh;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Users.Commands.ChangePassword;

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

        return Ok(ApiResponse<object>.Ok(result.Data!));
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

    // Cambio de contrasena del propio usuario autenticado (no requiere permiso de administracion, solo estar logueado)
    [HttpPatch("me/password")]
    [Authorize]
    public async Task<IActionResult> ChangeOwnPassword([FromBody] ChangeOwnPasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _mediator.Send(new ChangeOwnPasswordCommand(userId, request.CurrentPassword, request.NewPassword), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }

    // Solicitud de cambio de contrasena: avisa a los gerentes, misma respuesta exista o no el email
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
        return Ok(ApiResponse.Ok("Si el email está registrado, se avisó al gerente para que te asigne una contraseña nueva."));
    }
}

public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record ChangeOwnPasswordRequest(string CurrentPassword, string NewPassword);
public record ForgotPasswordRequest(string Email);
