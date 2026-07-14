using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.TechnicalDocuments.DTOs;

namespace SIGMA.Application.TechnicalDocuments.Queries.GetById;

public record GetTechnicalDocumentByIdQuery(Guid Id) : IRequest<Result<TechnicalDocumentDto>>;
