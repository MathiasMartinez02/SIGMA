using SIGMA.Domain.Enums;

namespace SIGMA.Application.Users.DTOs;

public class UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public string? LicenseNumber { get; init; }
    public DateTime? LicenseExpiry { get; init; }
    public string? Phone { get; init; }
    public string? AvatarUrl { get; init; }
    public bool IsActive { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public DateTime CreatedAt { get; init; }
}
