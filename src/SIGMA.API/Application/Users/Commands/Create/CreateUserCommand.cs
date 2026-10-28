using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Users.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.Users.Commands.Create;

public record CreateUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role,
    string? LicenseNumber,
    string? Phone
) : IRequest<Result<UserDto>>;
