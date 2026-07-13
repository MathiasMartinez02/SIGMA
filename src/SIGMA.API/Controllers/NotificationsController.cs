using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Notifications.Commands.MarkAllAsRead;
using SIGMA.Application.Notifications.Commands.MarkAsRead;
using SIGMA.Application.Notifications.Queries.GetAll;
using SIGMA.Application.Notifications.Queries.GetUnreadCount;

namespace SIGMA.API.Controllers;

// Expone las notificaciones del usuario autenticado: listado paginado, contador de no leidas y marcado de lectura
[ApiController]
[Route("api/v1/notifications")]
[Authorize]
[Tags("Notifications")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public NotificationsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    // Lista, paginadas y ordenadas por fecha descendente, las notificaciones del usuario autenticado
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? unreadOnly = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetNotificationsQuery(_currentUser.UserId!.Value, page, pageSize, unreadOnly),
            cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    // Devuelve solo la cantidad de notificaciones no leidas del usuario autenticado, para el badge de la campana
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        var count = await _mediator.Send(new GetUnreadNotificationCountQuery(_currentUser.UserId!.Value), cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { count }));
    }

    // Marca una notificacion puntual como leida, validando que pertenezca al usuario autenticado
    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new MarkNotificationAsReadCommand(id, _currentUser.UserId!.Value), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok("Notificacion marcada como leida."));
    }

    // Marca todas las notificaciones no leidas del usuario autenticado como leidas
    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new MarkAllNotificationsAsReadCommand(_currentUser.UserId!.Value), cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse.Ok("Notificaciones marcadas como leidas."));
    }
}
