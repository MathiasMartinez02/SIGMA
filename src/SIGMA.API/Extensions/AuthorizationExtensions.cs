using Microsoft.AspNetCore.Authorization;

namespace SIGMA.API.Extensions;

public static class AuthorizationExtensions
{
    public static void AddSigmaAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("CanManageWorkOrders", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("permissions", p => p.Contains("canManageWorkOrders"))));

            options.AddPolicy("CanApproveInspections", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("permissions", p => p.Contains("canApproveInspections"))));

            options.AddPolicy("CanManageAircraft", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("permissions", p => p.Contains("canManageAircraft"))));

            options.AddPolicy("CanManageClients", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("permissions", p => p.Contains("canManageClients"))));

            options.AddPolicy("CanManageInventory", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("permissions", p => p.Contains("canManageInventory"))));

            options.AddPolicy("CanManageUsers", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("permissions", p => p.Contains("canManageUsers"))));

            options.AddPolicy("CanViewReports", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("permissions", p => p.Contains("canViewReports"))));

            options.AddPolicy("CanViewDashboard", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("permissions", p => p.Contains("canViewDashboard"))));

            // Fase 4: alta de documentacion tecnica (manuales/boletines/directivas AD) reusa el permiso ya existente canSignDocuments
            options.AddPolicy("CanSignDocuments", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("permissions", p => p.Contains("canSignDocuments"))));
        });
    }

    private static bool HasClaim(this System.Security.Claims.ClaimsPrincipal user, string claimType, Func<string, bool> predicate)
    {
        var claim = user.FindFirst(claimType);
        return claim is not null && predicate(claim.Value);
    }
}
