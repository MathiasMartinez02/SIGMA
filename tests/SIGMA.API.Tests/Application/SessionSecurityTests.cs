using System.Security.Claims;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using SIGMA.API.Auth.Commands.ForgotPassword;
using SIGMA.API.Auth.Commands.Refresh;
using SIGMA.API.Common.Behaviours;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Entities;
using SIGMA.API.Enums;
using SIGMA.API.Extensions;
using SIGMA.API.Persistence;
using SIGMA.API.Users.Commands.ChangePassword;
using SIGMA.API.Users.Commands.ResetPassword;
using Xunit;

namespace SIGMA.Tests.Application;

public class SessionSecurityTests
{
    private readonly ApplicationDbContext _context = TestDb.Create();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtService _jwt = Substitute.For<IJwtService>();

    public SessionSecurityTests()
    {
        _hasher.Hash(Arg.Any<string>()).Returns(ci => $"hash:{ci.Arg<string>()}");
        _hasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(ci => ci.ArgAt<string>(1) == $"hash:{ci.ArgAt<string>(0)}");
        _jwt.RefreshTokenExpirationDays.Returns(30);
        _jwt.GenerateRefreshToken().Returns(_ => Guid.NewGuid().ToString("N"));
        _jwt.GenerateAccessToken(Arg.Any<User>()).Returns("access");
    }

    private async Task<User> AddUserAsync(string email, UserRole role, params string[] activeTokens)
    {
        var user = User.Create(email, "hash:sigma2026", "Nombre", "Apellido", role);
        _context.Users.Add(user);
        foreach (var token in activeTokens)
            _context.RefreshTokens.Add(RefreshToken.Create(user.Id, token, 30));
        await _context.SaveChangesAsync();
        return user;
    }

    private Task<int> ActiveTokensAsync(Guid userId) =>
        _context.RefreshTokens.CountAsync(t => t.UserId == userId && t.RevokedAt == null);

    [Fact]
    public void El_log_enmascara_contraseñas_y_tokens()
    {
        var masked = SensitiveDataMasker.Mask(new { Email = "a@b.com", Password = "sigma2026", RefreshToken = "abc", NewPassword = "x" });

        masked["Email"].Should().Be("a@b.com");
        masked["Password"].Should().Be("***");
        masked["RefreshToken"].Should().Be("***");
        masked["NewPassword"].Should().Be("***");
    }

    [Fact]
    public async Task Olvide_mi_contraseña_avisa_a_los_gerentes_sin_revelar_si_el_email_existe()
    {
        var manager = await AddUserAsync("gerente@sigma.aero", UserRole.Gerente);
        await AddUserAsync("mecanico@sigma.aero", UserRole.Mecanico);
        var handler = new ForgotPasswordCommandHandler(_context);

        var unknown = await handler.Handle(new ForgotPasswordCommand("noexiste@sigma.aero"), default);
        var known = await handler.Handle(new ForgotPasswordCommand("Mecanico@sigma.aero"), default);
        await handler.Handle(new ForgotPasswordCommand("mecanico@sigma.aero"), default);

        unknown.Succeeded.Should().BeTrue();
        known.Succeeded.Should().BeTrue();
        var notifications = await _context.Notifications.ToListAsync();
        notifications.Should().ContainSingle().Which.UserId.Should().Be(manager.Id);
        notifications[0].Type.Should().Be(NotificationType.SolicitudCambioPassword);
    }

    [Fact]
    public async Task El_gerente_asigna_contraseña_nueva_cierra_sesiones_y_resuelve_la_solicitud()
    {
        await AddUserAsync("gerente@sigma.aero", UserRole.Gerente);
        var user = await AddUserAsync("mecanico@sigma.aero", UserRole.Mecanico, "t1", "t2");
        await new ForgotPasswordCommandHandler(_context).Handle(new ForgotPasswordCommand(user.Email), default);

        var result = await new ResetUserPasswordCommandHandler(_context, _hasher)
            .Handle(new ResetUserPasswordCommand(user.Id, "NuevaClave123"), default);

        result.Succeeded.Should().BeTrue();
        user.PasswordHash.Should().Be("hash:NuevaClave123");
        (await ActiveTokensAsync(user.Id)).Should().Be(0);
        (await _context.Notifications.AnyAsync(n => !n.IsRead)).Should().BeFalse();
    }

    [Fact]
    public async Task Cambiar_la_contraseña_propia_cierra_las_sesiones()
    {
        var user = await AddUserAsync("inspector@sigma.aero", UserRole.Inspector, "t1");

        var result = await new ChangeOwnPasswordCommandHandler(_context, _hasher)
            .Handle(new ChangeOwnPasswordCommand(user.Id, "sigma2026", "OtraClave123"), default);

        result.Succeeded.Should().BeTrue();
        (await ActiveTokensAsync(user.Id)).Should().Be(0);
    }

    [Fact]
    public async Task Refresh_rota_el_token()
    {
        var user = await AddUserAsync("jefe@sigma.aero", UserRole.JefeTaller, "viejo");

        var result = await new RefreshTokenCommandHandler(_context, _jwt).Handle(new RefreshTokenCommand("viejo"), default);

        result.Succeeded.Should().BeTrue();
        result.Data!.RefreshToken.Should().NotBe("viejo");
        (await _context.RefreshTokens.SingleAsync(t => t.Token == "viejo")).IsRevoked.Should().BeTrue();
        (await ActiveTokensAsync(user.Id)).Should().Be(1);
    }

    [Fact]
    public async Task Reusar_un_token_ya_rotado_cierra_todas_las_sesiones()
    {
        var user = await AddUserAsync("jefe@sigma.aero", UserRole.JefeTaller, "viejo");
        var handler = new RefreshTokenCommandHandler(_context, _jwt);
        await handler.Handle(new RefreshTokenCommand("viejo"), default);

        var reuse = await handler.Handle(new RefreshTokenCommand("viejo"), default);

        reuse.Succeeded.Should().BeFalse();
        (await ActiveTokensAsync(user.Id)).Should().Be(0);
    }

    [Theory]
    [InlineData("canManageClients", true)]
    [InlineData("canManageWorkOrders", true)]
    [InlineData("canViewReports", false)]
    public void Policy_compuesta_acepta_cualquiera_de_los_permisos(string granted, bool expected)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("permissions", JsonSerializer.Serialize(new[] { "canViewDashboard", granted }))], "test"));

        AuthorizationExtensions.HasAnyPermission(principal, ["canManageWorkOrders", "canManageClients"])
            .Should().Be(expected);
    }

    [Fact]
    public void Policy_no_confunde_subcadenas_del_claim()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("permissions", "[\"canManageWorkOrdersX\"]")], "test"));

        AuthorizationExtensions.HasAnyPermission(principal, ["canManageWorkOrders"]).Should().BeFalse();
    }
}
