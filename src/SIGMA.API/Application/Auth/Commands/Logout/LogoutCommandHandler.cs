using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public LogoutCommandHandler(IApplicationDbContext context) => _context = context;

    // Cierre de sesion: revoca todos los refresh tokens del usuario
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await RefreshTokenRevoker.RevokeAllAsync(_context, request.UserId, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
