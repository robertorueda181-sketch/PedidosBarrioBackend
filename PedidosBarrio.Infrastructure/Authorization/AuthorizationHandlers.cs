using Microsoft.AspNetCore.Authorization;
using PedidosBarrio.Application.Services;
using System.Security.Claims;

namespace PedidosBarrio.Infrastructure.Authorization
{
    public class SubscriptionAuthorizationHandler : AuthorizationHandler<SubscriptionRequirement>
    {
        private readonly ISubscriptionAuthorizationService _subscriptionService;

        public SubscriptionAuthorizationHandler(ISubscriptionAuthorizationService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SubscriptionRequirement requirement)
        {
            // Obtener el ID del usuario desde los claims
            var userIdClaim = context.User.FindFirst("userId") ?? context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return; // Fail - no user ID
            }

            // Si permite admin y el usuario es admin, aprobar
            if (requirement.AllowAdmin && await _subscriptionService.IsAdminAsync(userId))
            {
                context.Succeed(requirement);
                return;
            }

            // Verificar suscripción
            if (await _subscriptionService.HasSubscriptionAccessAsync(userId, requirement.RequiredSubscription))
            {
                context.Succeed(requirement);
            }
        }
    }

    public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
    {
        private readonly ISubscriptionAuthorizationService _subscriptionService;

        public RoleAuthorizationHandler(ISubscriptionAuthorizationService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst("userId") ?? context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return;
            }

            if (await _subscriptionService.HasRoleAccessAsync(userId, requirement.RequiredRole))
            {
                context.Succeed(requirement);
            }
        }
    }

    public class SubscriptionOrRoleAuthorizationHandler : AuthorizationHandler<SubscriptionOrRoleRequirement>
    {
        private readonly ISubscriptionAuthorizationService _subscriptionService;

        public SubscriptionOrRoleAuthorizationHandler(ISubscriptionAuthorizationService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SubscriptionOrRoleRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst("userId") ?? context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return;
            }

            // Verificar si tiene la suscripción requerida O el rol requerido
            var hasSubscription = await _subscriptionService.HasSubscriptionAccessAsync(userId, requirement.RequiredSubscription);
            var hasRole = await _subscriptionService.HasRoleAccessAsync(userId, requirement.RequiredRole);

            if (hasSubscription || hasRole)
            {
                context.Succeed(requirement);
            }
        }
    }
}