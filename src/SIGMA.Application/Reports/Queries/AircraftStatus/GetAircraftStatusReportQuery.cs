using MediatR;
using SIGMA.Application.Reports.DTOs;

namespace SIGMA.Application.Reports.Queries.AircraftStatus;

public record GetAircraftStatusReportQuery() : IRequest<AircraftStatusReportDto>;
