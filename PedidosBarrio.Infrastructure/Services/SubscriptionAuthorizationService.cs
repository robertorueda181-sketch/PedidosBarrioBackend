using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Enums;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Infrastructure.Services
{
    public class SubscriptionAuthorizationService : ISubscriptionAuthorizationService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IApplicationLogger _logger;
        // Comentado temporalmente para evitar errores de compilación
        // private readonly ISuscripcionRepository _suscripcionRepository;

        public SubscriptionAuthorizationService(
            IUsuarioRepository usuarioRepository,
            IApplicationLogger logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
            // Comentado temporalmente para evitar errores
            // _suscripcionRepository = suscripcionRepository;
        }

        public async Task<TipoSuscripcion> GetUserSubscriptionAsync(Guid userId)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(userId);
                if (usuario == null)
                {
                    await _logger.LogWarningAsync($"Usuario no encontrado: {userId}", "SubscriptionAuthorizationService");
                    return TipoSuscripcion.Free; // Default a Free si no existe
                }

                // Implementación temporal - por ahora todos los usuarios tienen Free
                // TODO: Implementar lógica real cuando tengas la tabla de suscripciones
                
                // Simular lógica basada en email para testing
                if (usuario.Email?.Contains("empresa", StringComparison.OrdinalIgnoreCase) == true)
                    return TipoSuscripcion.Empresa;
                
                if (usuario.Email?.Contains("admin", StringComparison.OrdinalIgnoreCase) == true)
                    return TipoSuscripcion.Empresa;

                if (usuario.Email?.Contains("vecino", StringComparison.OrdinalIgnoreCase) == true)
                    return TipoSuscripcion.Vecino;

                // Default a Free
                return TipoSuscripcion.Free;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error obteniendo suscripción del usuario {userId}: {ex.Message}", ex, "SubscriptionAuthorizationService");
                return TipoSuscripcion.Free; // Fail-safe a Free
            }
        }

        public async Task<UsuarioRol[]> GetUserRolesAsync(Guid userId)
        {
            try
            {
                // Por ahora retorno un rol básico, pero puedes implementar un sistema complejo
                var usuario = await _usuarioRepository.GetByIdAsync(userId);
                if (usuario == null)
                    return [UsuarioRol.Usuario];

                // Aquí implementarías la lógica para obtener roles del usuario
                // Podría ser de una tabla UserRoles, o un campo en Usuario, etc.
                
                // Lógica temporal: si el email contiene "admin", es admin
                if (usuario.Email?.Contains("admin", StringComparison.OrdinalIgnoreCase) == true)
                    return [UsuarioRol.Admin];

                return [UsuarioRol.Usuario];
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error obteniendo roles del usuario {userId}: {ex.Message}", ex, "SubscriptionAuthorizationService");
                return [UsuarioRol.Usuario]; // Fail-safe a Usuario básico
            }
        }

        public async Task<bool> HasSubscriptionAccessAsync(Guid userId, TipoSuscripcion requiredSubscription)
        {
            var userSubscription = await GetUserSubscriptionAsync(userId);
            
            // Lógica de acceso por suscripción
            return requiredSubscription switch
            {
                TipoSuscripcion.Free => true, // Free siempre tiene acceso a funciones Free
                TipoSuscripcion.Vecino => userSubscription >= TipoSuscripcion.Vecino,
                TipoSuscripcion.Empresa => userSubscription == TipoSuscripcion.Empresa || userSubscription == TipoSuscripcion.Prueba,
                TipoSuscripcion.Prueba => userSubscription == TipoSuscripcion.Prueba,
                _ => false
            };
        }

        public async Task<bool> HasRoleAccessAsync(Guid userId, UsuarioRol requiredRole)
        {
            var userRoles = await GetUserRolesAsync(userId);
            return userRoles.Any(role => role >= requiredRole);
        }

        public async Task<bool> IsAdminAsync(Guid userId)
        {
            var roles = await GetUserRolesAsync(userId);
            return roles.Any(role => role.IsAdmin());
        }
    }
}