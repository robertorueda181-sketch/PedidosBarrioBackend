namespace PedidosBarrio.Domain.Entities
{
    public class Categoria
    {
        public short Categoria_ID { get; set; }
        public Guid EmpresaID { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public bool Activo { get; set; }
        public bool Mostrar { get; set; }

        public Categoria(short categoriaId, Guid empresaId, string descripcion, string codigo, bool activo = true, bool mostrar = false)
        {
            Categoria_ID = categoriaId;
            EmpresaID = empresaId;
            Descripcion = descripcion;
            Codigo = codigo;
            Activo = activo;
            Mostrar = mostrar;
        }

        private Categoria() { }
    }
}
