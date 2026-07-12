using SIGMA.Domain.Common;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Domain.Entities;

public class User : AuditableEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public UserRole Role { get; private set; }
    public string? LicenseNumber { get; private set; }
    public DateTime? LicenseExpiry { get; private set; }
    public string? Phone { get; private set; }
    public string? AvatarUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }

    public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];
    public ICollection<WorkOrder> CreatedWorkOrders { get; private set; } = [];

    private User() { }

    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        UserRole role,
        string? licenseNumber = null,
        string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("El email es requerido.");
        if (string.IsNullOrWhiteSpace(firstName)) throw new DomainException("El nombre es requerido.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new DomainException("El apellido es requerido.");

        return new User
        {
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Role = role,
            LicenseNumber = licenseNumber,
            Phone = phone
        };
    }

    public void UpdateProfile(string firstName, string lastName, string? phone, string? licenseNumber, DateTime? licenseExpiry, string? avatarUrl)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Phone = phone;
        LicenseNumber = licenseNumber;
        LicenseExpiry = licenseExpiry;
        AvatarUrl = avatarUrl;
    }

    public void ChangeRole(UserRole role) => Role = role;
    public void SetActive(bool isActive) => IsActive = isActive;
    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;
    public void UpdatePasswordHash(string hash) => PasswordHash = hash;

    public IList<string> GetPermissions() => Role switch
    {
        UserRole.Gerente => [
            "canViewDashboard", "canManageWorkOrders", "canApproveInspections",
            "canViewReports", "canManageAircraft", "canManageClients",
            "canManageInventory", "canManageUsers", "canSignDocuments"
        ],
        UserRole.OficinaTecnica => [
            "canViewDashboard", "canManageWorkOrders", "canViewReports",
            "canManageAircraft", "canManageClients"
        ],
        UserRole.Inspector => [
            "canViewDashboard", "canApproveInspections", "canViewReports", "canSignDocuments"
        ],
        UserRole.JefeTaller => [
            "canViewDashboard", "canManageWorkOrders", "canManageInventory", "canViewReports"
        ],
        UserRole.Mecanico => ["canViewDashboard"],
        UserRole.Panol => ["canViewDashboard", "canManageInventory"],
        UserRole.Administracion => ["canViewDashboard", "canViewReports", "canManageClients"],
        _ => []
    };
}
