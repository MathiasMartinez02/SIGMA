using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace SIGMA.API.Extensions;

public static class AuthorizationExtensions
{
    // Policies por permiso; las compuestas aceptan cualquiera de los permisos indicados
    public static void AddSigmaAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPermissionPolicy("CanManageWorkOrders", "canManageWorkOrders");
            options.AddPermissionPolicy("CanApproveInspections", "canApproveInspections");
            options.AddPermissionPolicy("CanManageAircraft", "canManageAircraft");
            options.AddPermissionPolicy("CanManageClients", "canManageClients");
            options.AddPermissionPolicy("CanManageInventory", "canManageInventory");
            options.AddPermissionPolicy("CanManageUsers", "canManageUsers");
            options.AddPermissionPolicy("CanViewReports", "canViewReports");
            options.AddPermissionPolicy("CanViewDashboard", "canViewDashboard");
            options.AddPermissionPolicy("CanSignDocuments", "canSignDocuments");
            options.AddPermissionPolicy("CanManageTechnicalDocuments", "canSignDocuments", "canManageAircraft");
            options.AddPermissionPolicy("CanManageAppointments", "canManageWorkOrders", "canManageClients");
        });
    }

    // Registra una policy que exige al menos uno de los permisos
    private static void AddPermissionPolicy(this AuthorizationOptions options, string name, params string[] anyOf) =>
        options.AddPolicy(name, policy => policy.RequireAssertion(ctx => HasAnyPermission(ctx.User, anyOf)));

    // Indica si el claim de permisos del token incluye alguno de los indicados
    public static bool HasAnyPermission(ClaimsPrincipal user, IEnumerable<string> anyOf)
    {
        var raw = user.FindFirst("permissions")?.Value;
        if (string.IsNullOrEmpty(raw)) return false;

        try
        {
            var granted = JsonSerializer.Deserialize<string[]>(raw) ?? [];
            return anyOf.Any(p => granted.Contains(p, StringComparer.Ordinal));
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
