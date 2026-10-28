using MediatR;
using SIGMA.API.Clients.DTOs;
using SIGMA.API.Common.Pagination;

namespace SIGMA.API.Clients.Queries.GetAll;

public record GetClientsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    bool? IsActive = null
) : IRequest<PaginatedResult<ClientDto>>;
