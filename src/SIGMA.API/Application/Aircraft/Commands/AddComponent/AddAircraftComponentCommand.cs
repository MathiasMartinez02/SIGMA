using MediatR;
using SIGMA.API.Aircraft.DTOs;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Aircraft.Commands.AddComponent;

public record AddAircraftComponentCommand(
    Guid AircraftId,
    string Name,
    string PartNumber,
    string SerialNumber,
    string Manufacturer,
    DateTime InstallDate,
    decimal InstallHours,
    decimal? LifeLimitHours,
    decimal? OverhaulDueHours
) : IRequest<Result<AircraftComponentDto>>;
