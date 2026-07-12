using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Users.Commands.Update;

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
