using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;
using SIGMA.API.Users.Queries.Lookup;

namespace SIGMA.API.Controllers;

[ApiController]
[Route("api/v1/users/lookup")]
[Authorize(Policy = "CanManageWorkOrders")]
[Tags("Users")]
public class UserLookupController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserLookupController(IMediator mediator) => _mediator = mediator;

    // Usuarios activos por rol para asignar a OT y tareas (no requiere gestionar usuarios)
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] UserRole[]? role, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserLookupQuery(role ?? []), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }
}
