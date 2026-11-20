using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using SIGMA.API.Identity;
using SIGMA.API.Services;
using SIGMA.API.Settings;
using Xunit;

namespace SIGMA.Tests.Application;

public class CurrentUserServiceTests
{
    private static readonly JwtSettings Settings = new()
    {
        SecretKey = "clave-de-prueba-de-al-menos-256-bits-para-hs256-0123456789",
        Issuer = "sigma.test",
        Audience = "sigma.test",
        AccessTokenExpirationHours = 1,
        RefreshTokenExpirationDays = 1
    };

    // Valida el token igual que el pipeline (con mapeo de claims entrantes)
    private static CurrentUserService ServiceFromToken(string token)
    {
        var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.SecretKey)),
            ValidIssuer = Settings.Issuer,
            ValidAudience = Settings.Audience
        }, out _);

        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = principal } };
        return new CurrentUserService(accessor);
    }

    [Fact]
    public void Lee_el_rol_del_token_aunque_el_claim_llegue_renombrado()
    {
        var user = User.Create("gerente@sigma.aero", "hash", "Carlos", "Rodriguez", UserRole.Gerente);
        var token = new JwtService(Options.Create(Settings)).GenerateAccessToken(user);

        var current = ServiceFromToken(token);

        current.Role.Should().Be("Gerente");
        current.UserId.Should().Be(user.Id);
    }
}
