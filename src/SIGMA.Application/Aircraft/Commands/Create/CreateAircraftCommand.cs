using MediatR;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Aircraft.Commands.Create;

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
