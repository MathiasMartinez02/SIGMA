using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Auth.Commands.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<Result<string>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken is null || !refreshToken.IsActive)
            return Result<string>.Failure("Refresh token inválido o expirado.");

        if (!refreshToken.User.IsActive)
            return Result<string>.Failure("El usuario está desactivado.");

        var newAccessToken = _jwtService.GenerateAccessToken(refreshToken.User);
        return Result<string>.Success(newAccessToken);
    }
}
