namespace SIGMA.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? Role { get; }
    string? FullName { get; }
    bool IsAuthenticated { get; }
    IList<string> Permissions { get; }
}
