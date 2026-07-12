using MediatR;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.Inspections.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Inspections.Queries.GetAll;

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
