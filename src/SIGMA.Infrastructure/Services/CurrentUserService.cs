using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SIGMA.Application.Common.Interfaces;

namespace SIGMA.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue("userId");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email)
        ?? User?.FindFirstValue("email");

    public string? Role => User?.FindFirstValue("role");

    public string? FullName => User?.FindFirstValue("fullName");

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public IList<string> Permissions
    {
        get
        {
            var raw = User?.FindFirstValue("permissions");
            if (string.IsNullOrEmpty(raw)) return [];
            try { return JsonSerializer.Deserialize<List<string>>(raw) ?? []; }
            catch { return []; }
        }
    }
}
