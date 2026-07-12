using MediatR;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Pagination;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Aircraft.Queries.GetAll;

public record GetAircraftQuery(
    int Page = 1,
    int PageSize = 10,
    AircraftStatus? Status = null,
    AircraftCategory? Category = null,
    Guid? ClientId = null,
    string? Search = null
) : IRequest<PaginatedResult<AircraftDto>>;
