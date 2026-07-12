using FluentAssertions;
using SIGMA.Application.Auth.Commands.Login;
using Xunit;

namespace SIGMA.Application.Tests;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Theory]
    [InlineData("gerente@sigma.aero", "sigma2024", true)]
    [InlineData("invalid-email", "sigma2024", false)]
    [InlineData("gerente@sigma.aero", "short", false)]
    [InlineData("", "sigma2024", false)]
    [InlineData("gerente@sigma.aero", "", false)]
    public async Task Validates_LoginCommand_Correctly(string email, string password, bool expectedValid)
    {
        var command = new LoginCommand(email, password);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().Be(expectedValid);
    }
}
