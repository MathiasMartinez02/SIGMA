using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGMA.Application.Common.Models;
using SIGMA.Application.TechnicalDocuments.Commands.Create;
using SIGMA.Application.TechnicalDocuments.Queries.GetAll;
using SIGMA.Application.TechnicalDocuments.Queries.GetById;
using SIGMA.Application.TechnicalDocuments.Queries.GetExpiring;
using SIGMA.Domain.Enums;

namespace SIGMA.API.Controllers;

// Fase 4: repositorio general de documentacion tecnica (manuales, boletines, directivas AD, certificados vigentes de toda la flota/el taller)
[ApiController]
[Route("api/v1/technical-documents")]
[Authorize]
[Tags("TechnicalDocuments")]
public class TechnicalDocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TechnicalDocumentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] TechnicalDocumentType? type = null,
        [FromQuery] string? search = null,
        [FromQuery] bool? expiringOnly = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetTechnicalDocumentsQuery(page, pageSize, type, search, expiringOnly), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    // Endpoint independiente del Dashboard para consultar documentos tecnicos por vencer (ventana de 30 dias)
    [HttpGet("expiring")]
    public async Task<IActionResult> GetExpiring(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetExpiringTechnicalDocumentsQuery(), cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTechnicalDocumentByIdQuery(id), cancellationToken);
        if (result.Failed) return NotFound(ApiResponse<object>.Fail(result.Errors));
        return Ok(ApiResponse<object>.Ok(result.Data!));
    }

    [HttpPost]
    [Authorize(Policy = "CanSignDocuments")]
    public async Task<IActionResult> Create([FromBody] CreateTechnicalDocumentCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (result.Failed) return BadRequest(ApiResponse<object>.Fail(result.Errors));
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<object>.Ok(result.Data));
    }
}
