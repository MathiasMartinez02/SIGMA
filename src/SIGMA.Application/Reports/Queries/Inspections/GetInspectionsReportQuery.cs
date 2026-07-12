using MediatR;
using SIGMA.Application.Reports.DTOs;

namespace SIGMA.Application.Reports.Queries.Inspections;

public record GetInspectionsReportQuery(
    DateTime DateFrom,
    DateTime DateTo,
    string? Type = null,
    Guid? InspectorId = null
) : IRequest<InspectionsReportDto>;
