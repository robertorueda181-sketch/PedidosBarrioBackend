using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.Login
{
    /// <summary>
    /// Command para login unificado
    /// Soporta: usuario/contraseña O Google
    /// </summary>
    public class LoginCommand : IRequest<LoginResponseDto>
    {
        // ===== LOGIN POR USUARIO/CONTRASEÑA =====
        public string Email { get; set; }
        public string Contrasena { get; set; }

        // ===== LOGIN POR GOOGLE =====
        public string Provider { get; set; } // "google" o null
        public string IdToken { get; set; }
        public string SocialId { get; set; }

        public LoginCommand() { }

        // Constructor para login por usuario/contraseña
        public LoginCommand(string email, string contrasena)
        {
            Email = email;
            Contrasena = contrasena;
            Provider = null;
        }

        // Constructor para login por Google
        public LoginCommand(string email, string provider, string idToken, string socialId = null)
        {
            Email = email;
            Provider = provider;
            IdToken = idToken;
            SocialId = socialId;
            Contrasena = null;
        }
    }
}

