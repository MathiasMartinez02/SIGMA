using MediatR;
using SIGMA.Application.Auth.DTOs;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponseDto>>;
