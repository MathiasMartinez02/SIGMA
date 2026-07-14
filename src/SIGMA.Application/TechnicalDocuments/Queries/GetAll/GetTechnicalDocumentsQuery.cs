using MediatR;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.TechnicalDocuments.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.TechnicalDocuments.Queries.GetAll;

public record GetTechnicalDocumentsQuery(
    int Page = 1,
    int PageSize = 10,
    TechnicalDocumentType? Type = null,
    string? Search = null,
    bool? ExpiringOnly = null
) : IRequest<PaginatedResult<TechnicalDocumentDto>>;
