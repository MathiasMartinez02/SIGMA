using MediatR;
using SIGMA.Application.Clients.DTOs;
using SIGMA.Application.Common.Pagination;

namespace SIGMA.Application.Clients.Queries.GetAll;

public record GetClientsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    bool? IsActive = null
) : IRequest<PaginatedResult<ClientDto>>;
