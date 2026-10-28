using MediatR;
using SIGMA.API.Reports.DTOs;

namespace SIGMA.API.Reports.Queries.AircraftStatus;

public record GetAircraftStatusReportQuery() : IRequest<AircraftStatusReportDto>;
