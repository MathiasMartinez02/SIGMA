using SIGMA.Domain.Common;

namespace SIGMA.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; private set; }
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, int expirationDays) =>
        new()
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expirationDays)
        };

    public void Revoke() => RevokedAt = DateTime.UtcNow;
}
