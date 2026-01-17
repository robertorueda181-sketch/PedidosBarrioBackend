using Microsoft.AspNetCore.Authorization;
using PedidosBarrio.Domain.Enums;

namespace PedidosBarrio.Infrastructure.Authorization
{
    // Requirement para verificar suscripción
    public class SubscriptionRequirement : IAuthorizationRequirement
    {
        public TipoSuscripcion RequiredSubscription { get; }
        public bool AllowAdmin { get; }

        public SubscriptionRequirement(TipoSuscripcion requiredSubscription, bool allowAdmin = true)
        {
            RequiredSubscription = requiredSubscription;
            AllowAdmin = allowAdmin;
        }
    }

    // Requirement para verificar roles
    public class RoleRequirement : IAuthorizationRequirement
    {
        public UsuarioRol RequiredRole { get; }

        public RoleRequirement(UsuarioRol requiredRole)
        {
            RequiredRole = requiredRole;
        }
    }

    // Requirement combinado (suscripción O rol)
    public class SubscriptionOrRoleRequirement : IAuthorizationRequirement
    {
        public TipoSuscripcion RequiredSubscription { get; }
        public UsuarioRol RequiredRole { get; }

        public SubscriptionOrRoleRequirement(TipoSuscripcion requiredSubscription, UsuarioRol requiredRole)
        {
            RequiredSubscription = requiredSubscription;
            RequiredRole = requiredRole;
        }
    }
}