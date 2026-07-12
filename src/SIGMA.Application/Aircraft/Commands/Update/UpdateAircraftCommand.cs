using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Aircraft.Commands.Update;

public record UpdateAircraftCommand(
    Guid Id,
    string Model,
    string Manufacturer,
    int Year,
    AircraftCategory Category,
    string SerialNumber,
    string EngineModel,
    string EngineSerialNumber,
    decimal TotalFlightHours,
    decimal TotalLandings,
    DateTime? LastInspectionDate,
    DateTime? NextInspectionDue,
    decimal NextInspectionHours,
    DateTime CertificateExpiry
) : IRequest<Result>;
