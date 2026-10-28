using SIGMA.API.Entities;

namespace SIGMA.API.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    long GetAccessTokenExpirationUnixTimestamp();
}
