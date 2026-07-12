using SIGMA.Domain.Entities;

namespace SIGMA.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    long GetAccessTokenExpirationUnixTimestamp();
}
