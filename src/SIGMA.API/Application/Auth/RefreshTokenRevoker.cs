using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;

namespace SIGMA.API.Auth;

public static class RefreshTokenRevoker
{
    // Revoca todas las sesiones activas del usuario (no guarda cambios)
    public static async Task RevokeAllAsync(IApplicationDbContext context, Guid userId, CancellationToken cancellationToken)
    {
        var activeTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
            token.Revoke();
    }
}
