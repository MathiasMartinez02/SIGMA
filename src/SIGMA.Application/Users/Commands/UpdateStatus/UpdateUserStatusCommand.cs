using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Users.Commands.UpdateStatus;

public record UpdateUserStatusCommand(Guid Id, bool IsActive) : IRequest<Result>;
