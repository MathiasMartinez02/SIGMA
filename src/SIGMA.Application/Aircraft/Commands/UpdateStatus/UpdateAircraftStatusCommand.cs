using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Aircraft.Commands.UpdateStatus;

public record UpdateAircraftStatusCommand(Guid Id, AircraftStatus Status) : IRequest<Result>;
