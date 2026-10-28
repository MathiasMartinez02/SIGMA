using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Users.Commands.UpdateStatus;

public record UpdateUserStatusCommand(Guid Id, bool IsActive) : IRequest<Result>;
