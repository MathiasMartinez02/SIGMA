using MediatR;
using SIGMA.API.Aircraft.DTOs;
using SIGMA.API.Common.Pagination;
using SIGMA.API.Enums;

namespace SIGMA.API.Aircraft.Queries.GetAll;

public record GetAircraftQuery(
    int Page = 1,
    int PageSize = 10,
    AircraftStatus? Status = null,
    AircraftCategory? Category = null,
    Guid? ClientId = null,
    string? Search = null
) : IRequest<PaginatedResult<AircraftDto>>;
