using PedidosBarrio.Domain.Enums;

namespace PedidosBarrio.Application.Services
{
    public interface ISubscriptionAuthorizationService
    {
        Task<TipoSuscripcion> GetUserSubscriptionAsync(Guid userId);
        Task<UsuarioRol[]> GetUserRolesAsync(Guid userId);
        Task<bool> HasSubscriptionAccessAsync(Guid userId, TipoSuscripcion requiredSubscription);
        Task<bool> HasRoleAccessAsync(Guid userId, UsuarioRol requiredRole);
        Task<bool> IsAdminAsync(Guid userId);
    }
}