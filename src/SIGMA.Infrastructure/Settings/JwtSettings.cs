namespace SIGMA.Infrastructure.Settings;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "sigma.aero";
    public string Audience { get; set; } = "sigma.frontend";
    public int AccessTokenExpirationHours { get; set; } = 8;
    public int RefreshTokenExpirationDays { get; set; } = 30;
}
