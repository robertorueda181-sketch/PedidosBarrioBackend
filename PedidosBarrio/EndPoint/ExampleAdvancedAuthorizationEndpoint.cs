using Microsoft.AspNetCore.Mvc;
using PedidosBarrio.Infrastructure.Authorization;
using PedidosBarrio.Domain.Enums;

namespace PedidosBarrio.Api.EndPoint
{
    public static class ExampleAdvancedAuthorizationEndpoint
    {
        public static void MapAdvancedAuthorizationExamples(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/Examples/Authorization")
                           .WithTags("Authorization Examples");

            // 1. Endpoint solo para plan Free (para testing)
            group.MapGet("/free-only", () =>
            {
                return Results.Ok(new { message = "Solo usuarios Free pueden ver esto (para testing)" });
            })
            .RequireAuthorization(AuthorizationPolicies.RequireFreePlan)
            .WithName("FreeOnlyExample");

            // 2. Endpoint solo para plan Vecino o superior
            group.MapGet("/vecino-plus", () =>
            {
                return Results.Ok(new { message = "Solo usuarios Vecino+ pueden ver esto" });
            })
            .RequireAuthorization(AuthorizationPolicies.RequireVecinoPlan)
            .WithName("VecinoPlusExample");

            // 3. Endpoint solo para plan Empresa
            group.MapGet("/empresa-only", () =>
            {
                return Results.Ok(new { message = "Solo usuarios Empresa pueden ver esto" });
            })
            .RequireAuthorization(AuthorizationPolicies.RequireEmpresaPlan)
            .WithName("EmpresaOnlyExample");

            // 4. Endpoint solo para Admins
            group.MapGet("/admin-only", () =>
            {
                return Results.Ok(new { message = "Solo Admins pueden ver esto" });
            })
            .RequireAuthorization(AuthorizationPolicies.RequireAdmin)
            .WithName("AdminOnlyExample");

            // 5. Endpoint combinado: Empresa O Admin
            group.MapGet("/empresa-or-admin", () =>
            {
                return Results.Ok(new { message = "Usuarios Empresa o Admins pueden ver esto" });
            })
            .RequireAuthorization(AuthorizationPolicies.EmpresaOrAdmin)
            .WithName("EmpresaOrAdminExample");

            // 6. Endpoint con acceso Premium (Empresa, Prueba o Admin)
            group.MapGet("/premium", () =>
            {
                return Results.Ok(new { message = "Acceso Premium: Empresa, Prueba o Admin" });
            })
            .RequireAuthorization(AuthorizationPolicies.PremiumAccess)
            .WithName("PremiumAccessExample");

            // 7. Endpoint público (sin autorización)
            group.MapGet("/public", () =>
            {
                return Results.Ok(new { message = "Este endpoint es público" });
            })
            .AllowAnonymous()
            .WithName("PublicExample");

            // 8. Endpoint con autorización específica por feature
            group.MapGet("/analytics", () =>
            {
                return Results.Ok(new { 
                    message = "Datos de Analytics",
                    data = new { users = 100, subscriptions = 50 }
                });
            })
            .RequireAuthorization(AuthorizationPolicies.FeatureAccess.ViewAnalytics)
            .WithName("AnalyticsExample");

            // 9. Info sobre las políticas disponibles
            group.MapGet("/policies-info", () =>
            {
                return Results.Ok(new
                {
                    subscriptionPolicies = new
                    {
                        free = AuthorizationPolicies.RequireFreePlan,
                        vecino = AuthorizationPolicies.RequireVecinoPlan,
                        empresa = AuthorizationPolicies.RequireEmpresaPlan,
                        prueba = AuthorizationPolicies.RequirePruebaPlan
                    },
                    rolePolicies = new
                    {
                        user = AuthorizationPolicies.RequireUser,
                        moderator = AuthorizationPolicies.RequireModerator,
                        admin = AuthorizationPolicies.RequireAdmin
                    },
                    combinedPolicies = new
                    {
                        empresaOrAdmin = AuthorizationPolicies.EmpresaOrAdmin,
                        vecinoOrModerator = AuthorizationPolicies.VecinoOrModerator,
                        premiumAccess = AuthorizationPolicies.PremiumAccess
                    },
                    featureAccess = AuthorizationPolicies.GetFeaturePolicies().ToDictionary(
                        kvp => kvp.Key,
                        kvp => new { subscription = kvp.Value.suscripcion.ToString(), role = kvp.Value.rol.ToString() }
                    )
                });
            })
            .AllowAnonymous()
            .WithName("PoliciesInfoExample");
        }
    }
}