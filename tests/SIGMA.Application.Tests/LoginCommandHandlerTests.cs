using Microsoft.EntityFrameworkCore;
using NSubstitute;
using FluentAssertions;
using SIGMA.Application.Auth.Commands.Login;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Domain.Entities;
using SIGMA.Domain.Enums;
using SIGMA.Infrastructure.Persistence;
using Xunit;

namespace SIGMA.Application.Tests;

public class LoginCommandHandlerTests
{
    private static ApplicationDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Handle_ValidCredentials_Returns_Success_With_Tokens()
    {
        // Arrange
        var context = CreateInMemoryContext(Guid.NewGuid().ToString());
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var jwtService = Substitute.For<IJwtService>();
        var dateTime = Substitute.For<IDateTimeProvider>();

        var hash = "hashed-password";
        passwordHasher.Hash("sigma2024").Returns(hash);
        passwordHasher.Verify("sigma2024", hash).Returns(true);
        jwtService.GenerateAccessToken(Arg.Any<User>()).Returns("access-token");
        jwtService.GenerateRefreshToken().Returns("refresh-token");
        jwtService.GetAccessTokenExpirationUnixTimestamp().Returns(9999999999L);
        dateTime.UtcNow.Returns(DateTime.UtcNow);

        var user = User.Create("gerente@sigma.aero", hash, "Carlos", "Rodríguez", UserRole.Gerente);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new LoginCommandHandler(context, jwtService, passwordHasher, dateTime);
        var command = new LoginCommand("gerente@sigma.aero", "sigma2024");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Tokens.AccessToken.Should().Be("access-token");
        result.Data.Tokens.RefreshToken.Should().Be("refresh-token");
        result.Data.User.Email.Should().Be("gerente@sigma.aero");
        result.Data.User.Role.Should().Be("Gerente");
    }

    [Fact]
    public async Task Handle_InvalidCredentials_Returns_Failure()
    {
        // Arrange
        var context = CreateInMemoryContext(Guid.NewGuid().ToString());
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var jwtService = Substitute.For<IJwtService>();
        var dateTime = Substitute.For<IDateTimeProvider>();

        passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var user = User.Create("gerente@sigma.aero", "hashed-pwd", "Carlos", "Rodríguez", UserRole.Gerente);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new LoginCommandHandler(context, jwtService, passwordHasher, dateTime);
        var command = new LoginCommand("gerente@sigma.aero", "wrong-password");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain("Credenciales inválidas.");
    }

    [Fact]
    public async Task Handle_InactiveUser_Returns_Failure()
    {
        // Arrange
        var context = CreateInMemoryContext(Guid.NewGuid().ToString());
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var jwtService = Substitute.For<IJwtService>();
        var dateTime = Substitute.For<IDateTimeProvider>();

        var hash = "hashed-password";
        passwordHasher.Verify("sigma2024", hash).Returns(true);

        var user = User.Create("gerente@sigma.aero", hash, "Carlos", "Rodríguez", UserRole.Gerente);
        user.SetActive(false);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new LoginCommandHandler(context, jwtService, passwordHasher, dateTime);
        var command = new LoginCommand("gerente@sigma.aero", "sigma2024");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("desactivado"));
    }
}
