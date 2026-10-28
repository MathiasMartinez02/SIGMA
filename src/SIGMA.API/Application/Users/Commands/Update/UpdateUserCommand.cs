using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;

namespace SIGMA.API.Users.Commands.Update;

public record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    UserRole Role,
    string? LicenseNumber,
    DateTime? LicenseExpiry,
    string? Phone,
    string? AvatarUrl
) : IRequest<Result>;
