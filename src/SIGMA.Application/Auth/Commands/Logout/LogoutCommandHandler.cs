using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public LogoutCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == request.UserId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
            token.Revoke();

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
