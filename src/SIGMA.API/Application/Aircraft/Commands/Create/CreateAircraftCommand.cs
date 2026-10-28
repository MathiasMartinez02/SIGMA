using MediatR;
using SIGMA.API.Aircraft.DTOs;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.Aircraft.Commands.Create;

public record CreateAircraftCommand(
    string Registration,
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
    DateTime CertificateExpiry,
    Guid ClientId
) : IRequest<Result<AircraftDto>>;
