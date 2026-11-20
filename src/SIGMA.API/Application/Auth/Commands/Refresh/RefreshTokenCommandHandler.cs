using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Entities;

namespace SIGMA.API.Auth.Commands.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshResultDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    // Rota el refresh token; si llega uno ya revocado se cierran todas las sesiones del usuario
    public async Task<Result<RefreshResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var current = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (current is null || current.IsExpired)
            return Result<RefreshResultDto>.Failure("Refresh token inválido o expirado.");

        if (current.IsRevoked)
        {
            await RefreshTokenRevoker.RevokeAllAsync(_context, current.UserId, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<RefreshResultDto>.Failure("Refresh token inválido o expirado.");
        }

        if (!current.User.IsActive || current.User.IsDeleted)
            return Result<RefreshResultDto>.Failure("El usuario está desactivado.");

        current.Revoke();
        var replacement = RefreshToken.Create(current.UserId, _jwtService.GenerateRefreshToken(),
            _jwtService.RefreshTokenExpirationDays);
        _context.RefreshTokens.Add(replacement);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<RefreshResultDto>.Success(
            new RefreshResultDto(_jwtService.GenerateAccessToken(current.User), replacement.Token));
    }
}
