namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// Request para registro completo
    /// </summary>
    public class RegisterSocialRequestDto
    {
        // DATOS DE USUARIO
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; } // null si es Google

        // DATOS DE EMPRESA
        public string NombreEmpresa { get; set; }
        public short TipoEmpresa { get; set; } // NEGOCIO, SERVICIO, INMUEBLE
        public string Categoria { get; set; } // null si es INMUEBLE
        public string Telefono { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }

        // LOGIN SOCIAL
        public string Provider { get; set; } // "google" o null
        public string SocialId { get; set; }
        public string IdToken { get; set; }
    }
}
