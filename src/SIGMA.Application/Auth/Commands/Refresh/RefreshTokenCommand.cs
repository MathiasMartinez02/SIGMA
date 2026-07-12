using MediatR;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Auth.Commands.Refresh;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<string>>;
