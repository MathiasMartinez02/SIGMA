using MediatR;
using SIGMA.API.Common.Pagination;
using SIGMA.API.TechnicalDocuments.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.TechnicalDocuments.Queries.GetAll;

public record GetTechnicalDocumentsQuery(
    int Page = 1,
    int PageSize = 10,
    TechnicalDocumentType? Type = null,
    string? Search = null,
    bool? ExpiringOnly = null
) : IRequest<PaginatedResult<TechnicalDocumentDto>>;
