using FluentAssertions;
using SIGMA.Domain.Exceptions;
using SIGMA.Domain.ValueObjects;
using Xunit;

namespace SIGMA.Domain.Tests;

public class RegistrationValueObjectTests
{
    [Theory]
    [InlineData("LV-RGT")]
    [InlineData("LV-ABCD")]
    [InlineData("OB-GXA")]
    [InlineData("PP-XGT")]
    public void Valid_Registration_Creates_Successfully(string value)
    {
        var act = () => Registration.Create(value);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("lv-rgt")]
    [InlineData("LV-ABCDE")]
    [InlineData("L-RGT")]
    [InlineData("LVR-GT")]
    [InlineData("")]
    [InlineData(null)]
    public void Invalid_Registration_Throws_DomainException(string? value)
    {
        var act = () => Registration.Create(value!);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Registration_Is_Normalized_To_Uppercase()
    {
        var reg = Registration.Create("lv-rgt");
        reg.Value.Should().Be("LV-RGT");
    }

    [Fact]
    public void Registration_Equality_Works()
    {
        var reg1 = Registration.Create("LV-RGT");
        var reg2 = Registration.Create("LV-RGT");
        reg1.Should().Be(reg2);
    }
}
