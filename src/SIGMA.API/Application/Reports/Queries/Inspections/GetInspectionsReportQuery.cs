using MediatR;
using SIGMA.API.Reports.DTOs;

namespace SIGMA.API.Reports.Queries.Inspections;

public record GetInspectionsReportQuery(
    DateTime DateFrom,
    DateTime DateTo,
    string? Type = null,
    Guid? InspectorId = null
) : IRequest<InspectionsReportDto>;
