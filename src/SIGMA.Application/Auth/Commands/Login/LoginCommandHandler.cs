using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Auth.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTime;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTime)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _dateTime = dateTime;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant() && !u.IsDeleted, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<LoginResponseDto>.Failure("Credenciales inválidas.");

        if (!user.IsActive)
            return Result<LoginResponseDto>.Failure("El usuario está desactivado. Contacte al administrador.");

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenValue = _jwtService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, refreshTokenValue, 30);

        _context.RefreshTokens.Add(refreshToken);
        user.RecordLogin();
        await _context.SaveChangesAsync(cancellationToken);

        var permissions = user.GetPermissions();

        return Result<LoginResponseDto>.Success(new LoginResponseDto
        {
            User = new UserTokenDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                Permissions = permissions,
                AvatarUrl = user.AvatarUrl,
                LicenseNumber = user.LicenseNumber,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt
            },
            Tokens = new TokensDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                ExpiresAt = _jwtService.GetAccessTokenExpirationUnixTimestamp()
            }
        });
    }
}
