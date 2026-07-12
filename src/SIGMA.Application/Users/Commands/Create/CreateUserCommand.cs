using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Users.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Users.Commands.Create;

public record CreateUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role,
    string? LicenseNumber,
    string? Phone
) : IRequest<Result<UserDto>>;
