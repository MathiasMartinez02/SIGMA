using MediatR;
using SIGMA.API.Common.Pagination;
using SIGMA.API.Inspections.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.Inspections.Queries.GetAll;

public record GetInspectionsQuery(
    int Page = 1,
    int PageSize = 10,
    InspectionStatus? Status = null,
    string? Type = null,
    Guid? InspectorId = null,
    Guid? AircraftId = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    string? Search = null
) : IRequest<PaginatedResult<InspectionDto>>;
