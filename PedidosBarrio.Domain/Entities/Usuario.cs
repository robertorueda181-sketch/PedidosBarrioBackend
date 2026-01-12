namespace PedidosBarrio.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string ContrasenaHash { get; set; }
        public string ContrasenaSalt { get; set; }
        public Guid EmpresaID { get; set; }
        
        // ===== SOCIAL LOGIN =====
        public string Provider { get; set; } // "google" o null
        public string SocialId { get; set; } // Google ID u otro provider ID

        public Usuario(string nombreUsuario, string email, string contrasenaHash, string contrasenaSalt)
        {
            ID = Guid.NewGuid();
            NombreUsuario = nombreUsuario;
            Email = email;
            ContrasenaHash = contrasenaHash;
            ContrasenaSalt = contrasenaSalt;
            Activa = true;
            FechaRegistro = DateTime.UtcNow;
        }

        private Usuario() { }
    }
}
