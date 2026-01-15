namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// DTO para login social (Google, Facebook, etc.)
    /// </summary>
    public class SocialLoginRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string IdToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de respuesta con JWT token
    /// </summary>
    public class SocialLoginResponseDto
    {
        public Guid UsuarioID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public Guid? EmpresaID { get; set; }
        public string Provider { get; set; } = string.Empty;
        public bool EsNuevoUsuario { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiracion { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public string TipoEmpresa { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para refresh token
    /// </summary>
    public class RefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para response de refresh token
    /// </summary>
    public class RefreshTokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiracion { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
