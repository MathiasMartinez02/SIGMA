using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.Aircraft.Commands.UpdateStatus;

public record UpdateAircraftStatusCommand(Guid Id, AircraftStatus Status) : IRequest<Result>;
