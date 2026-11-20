using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Auth.Commands.Refresh;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<RefreshResultDto>>;

public record RefreshResultDto(string AccessToken, string RefreshToken);
