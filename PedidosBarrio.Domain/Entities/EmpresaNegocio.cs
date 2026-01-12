namespace PedidosBarrio.Domain.Entities
{
    public class Empresa: BaseEntity
    {
        public Guid UsuarioID { get; set; }
        public short TipoEmpresa { get; set; }


        public Empresa(Guid usuarioID,short tipoEmpresa)
        {
            ID = Guid.NewGuid();
            usuarioID = UsuarioID;
            TipoEmpresa = tipoEmpresa;
            Activa = true;
            FechaRegistro = DateTime.UtcNow;

        }

        private Empresa() { }
    }
}
