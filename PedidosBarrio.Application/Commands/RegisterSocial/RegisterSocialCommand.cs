using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.RegisterSocial
{
    /// <summary>
    /// Command para registro completo con empresa (usuario/contraseña o Google)
    /// Incluye: datos usuario, empresa, categoría, teléfono
    /// </summary>
    public class RegisterSocialCommand : IRequest<LoginResponseDto>
    {
        // ===== DATOS DE USUARIO =====
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; } // Null/empty si es Google
        
        // ===== DATOS DE EMPRESA =====
        public string NombreEmpresa { get; set; }
        public short TipoEmpresa { get; set; } // 1 NEGOCIO, 2 SERVICIO, 3 INMUEBLE
        public string Categoria { get; set; } // Para NEGOCIO y SERVICIO
        public string Telefono { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        
        // ===== LOGIN SOCIAL (GOOGLE) =====
        public string Provider { get; set; } // "google" o null si es usuario/contraseña
        public string SocialId { get; set; }
        public string IdToken { get; set; }

        public RegisterSocialCommand() { }

        public RegisterSocialCommand(
            string email,
            string nombre,
            string apellido,
            string nombreUsuario,
            string contrasena,
            string nombreEmpresa,
            short tipoEmpresa,
            string categoria,
            string telefono,
            string descripcion,
            string direccion,
            string referencia,
            string provider = null,
            string socialId = null,
            string idToken = null)
        {
            Email = email;
            Nombre = nombre;
            Apellido = apellido;
            NombreUsuario = nombreUsuario;
            Contrasena = contrasena;
            NombreEmpresa = nombreEmpresa;
            TipoEmpresa = tipoEmpresa;
            Categoria = categoria;
            Telefono = telefono;
            Descripcion = descripcion;
            Direccion = direccion;
            Referencia = referencia;
            Provider = provider;
            SocialId = socialId;
            IdToken = idToken;
        }
    }
}
