using MediatR;
using SIGMA.API.Auth.DTOs;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponseDto>>;
