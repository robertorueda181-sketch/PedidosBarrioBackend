namespace PedidosBarrio.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string ContrasenaHash { get; set; }
        public string ContrasenaSalt { get; set; }
        public Guid EmpresaID { get; set; }

        public Usuario(string nombreUsuario, string email, string contrasenaHash, string contrasenaSalt, Guid empresaID)
        {
            ID = Guid.NewGuid();
            NombreUsuario = nombreUsuario;
            Email = email;
            ContrasenaHash = contrasenaHash;
            ContrasenaSalt = contrasenaSalt;
            EmpresaID = empresaID;
        }

        private Usuario() { }
    }
}
