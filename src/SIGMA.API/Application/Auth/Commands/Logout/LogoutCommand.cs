using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Auth.Commands.Logout;

public record LogoutCommand(Guid UserId) : IRequest<Result>;
