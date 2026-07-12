using MediatR;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Aircraft.Commands.AddComponent;

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
