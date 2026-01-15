using Microsoft.AspNetCore.Http;
using PedidosBarrio.Application.Services;
using System.Security.Claims;

namespace PedidosBarrio.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetEmpresaId()
        {
            var empresaIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("EmpresaID")?.Value;
            if (string.IsNullOrEmpty(empresaIdClaim) || !Guid.TryParse(empresaIdClaim, out var empresaId))
            {
                throw new UnauthorizedAccessException("No se pudo obtener el EmpresaID del token");
            }
            return empresaId;
        }

        public Guid GetUsuarioId()
        {
            // Primero intentar con "UsuarioID", luego con NameIdentifier
            var usuarioIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UsuarioID")?.Value
                                ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                                
            if (string.IsNullOrEmpty(usuarioIdClaim) || !Guid.TryParse(usuarioIdClaim, out var usuarioId))
            {
                throw new UnauthorizedAccessException("No se pudo obtener el UsuarioID del token");
            }
            return usuarioId;
        }

        public string GetUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value 
                   ?? throw new UnauthorizedAccessException("No se pudo obtener el email del token");
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }
}