namespace SIGMA.Application.Auth.DTOs;

public class LoginResponseDto
{
    public UserTokenDto User { get; init; } = null!;
    public TokensDto Tokens { get; init; } = null!;
}

public class UserTokenDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public IList<string> Permissions { get; init; } = [];
    public string? AvatarUrl { get; init; }
    public string? LicenseNumber { get; init; }
    public bool IsActive { get; init; }
    public DateTime? LastLoginAt { get; init; }
}

public class TokensDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public long ExpiresAt { get; init; }
}
