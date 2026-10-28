using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.Aircraft.Commands.Update;

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
