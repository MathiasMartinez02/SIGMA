using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Auth.Commands.Refresh;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<string>>;
