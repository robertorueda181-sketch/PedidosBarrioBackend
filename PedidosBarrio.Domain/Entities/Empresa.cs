namespace PedidosBarrio.Domain.Entities
{
    public class Empresa: BaseEntity
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Email { get; set; }
        public string ContrasenaHash { get; set; }
        public string ContrasenaSalt { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }

        public int UsuarioID { get; set; }

        public Empresa(string nombre, string email, string contrasenaHash,string contrasenaSalt, string telefono)
        {
            ID = Guid.NewGuid();
            Nombre = nombre;
            Email = email;
            ContrasenaHash = contrasenaHash;
            ContrasenaSalt = contrasenaSalt;
            Telefono = telefono;
        }

        private Empresa() { }
    }
}
