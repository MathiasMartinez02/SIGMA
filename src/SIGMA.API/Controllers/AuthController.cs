using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Auth.Commands.ForgotPassword;
using SIGMA.Application.Auth.Commands.Login;
using SIGMA.Application.Auth.Commands.Logout;
using SIGMA.Application.Auth.Commands.Refresh;
using SIGMA.Application.Auth.Commands.ResetPassword;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Users.Commands.ChangePassword;

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

    // Paso 1 del flujo de recuperacion de contrasena: genera un token de un solo uso.
    // MVP sin envio de email real (no hay infraestructura de correo en el proyecto): el token se devuelve
    // en la respuesta en vez de enviarse por mail, decision de alcance documentada en PLAN_ANALISIS_FUNCIONAL.md.
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ForgotPasswordCommand(request.Email), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    // Paso 2 del flujo de recuperacion: aplica la nueva contrasena usando el token generado en el paso 1
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ResetPasswordCommand(request.Token, request.NewPassword), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok());
    }
}

public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record ChangeOwnPasswordRequest(string CurrentPassword, string NewPassword);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword);
