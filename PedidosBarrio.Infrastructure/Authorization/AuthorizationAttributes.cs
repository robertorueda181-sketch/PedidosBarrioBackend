using Microsoft.AspNetCore.Authorization;
using PedidosBarrio.Domain.Enums;

namespace PedidosBarrio.Infrastructure.Authorization
{
    // Atributo para requerir suscripción específica
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireSubscriptionAttribute : AuthorizeAttribute
    {
        public RequireSubscriptionAttribute(TipoSuscripcion requiredSubscription)
        {
            Policy = GetPolicyName(requiredSubscription);
        }

        private static string GetPolicyName(TipoSuscripcion subscription)
        {
            return subscription switch
            {
                TipoSuscripcion.Free => AuthorizationPolicies.RequireFreePlan,
                TipoSuscripcion.Vecino => AuthorizationPolicies.RequireVecinoPlan,
                TipoSuscripcion.Empresa => AuthorizationPolicies.RequireEmpresaPlan,
                TipoSuscripcion.Prueba => AuthorizationPolicies.RequirePruebaPlan,
                _ => throw new ArgumentException($"Suscripción no soportada: {subscription}")
            };
        }
    }

    // Atributo para requerir rol específico
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireRoleAttribute : AuthorizeAttribute
    {
        public RequireRoleAttribute(UsuarioRol requiredRole)
        {
            Policy = GetPolicyName(requiredRole);
        }

        private static string GetPolicyName(UsuarioRol role)
        {
            return role switch
            {
                UsuarioRol.Usuario => AuthorizationPolicies.RequireUser,
                UsuarioRol.Moderador => AuthorizationPolicies.RequireModerator,
                UsuarioRol.Admin => AuthorizationPolicies.RequireAdmin,
                _ => throw new ArgumentException($"Rol no soportado: {role}")
            };
        }
    }

    // Atributo para features específicas
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireFeatureAccessAttribute : AuthorizeAttribute
    {
        public RequireFeatureAccessAttribute(string featureName)
        {
            Policy = featureName;
        }
    }

    // Atributo combinado para casos comunes
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PremiumAccessAttribute : AuthorizeAttribute
    {
        public PremiumAccessAttribute()
        {
            Policy = AuthorizationPolicies.PremiumAccess;
        }
    }
}