using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Auth.Commands.Logout;

public record LogoutCommand(Guid UserId) : IRequest<Result>;
