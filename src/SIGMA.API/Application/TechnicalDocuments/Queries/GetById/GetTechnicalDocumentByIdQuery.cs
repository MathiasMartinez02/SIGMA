using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.TechnicalDocuments.DTOs;

namespace SIGMA.API.TechnicalDocuments.Queries.GetById;

public record GetTechnicalDocumentByIdQuery(Guid Id) : IRequest<Result<TechnicalDocumentDto>>;
