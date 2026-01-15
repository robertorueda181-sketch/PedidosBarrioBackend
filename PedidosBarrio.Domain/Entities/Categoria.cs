namespace PedidosBarrio.Domain.Entities
{
    public class Categoria
    {
        public short CategoriaID { get; set; }
        public Guid EmpresaID { get; set; }
        public string Descripcion { get; set; }
        public string Color { get; set; }
        public bool Activo { get; set; }

        public Categoria(Guid empresaId, string descripcion, string color)
        {
            EmpresaID = empresaId;
            Descripcion = descripcion;
            Color = color;
            Activo = true; // Default true as requested
        }
        public Categoria(string descripcion, string color)
        {
            Descripcion = descripcion;
            Color = color;
            Activo = true; // Default true as requested
        }
        // Constructor for mapping from database
        public Categoria(short categoriaId, Guid empresaId, string descripcion, string color, bool activo)
        {
            CategoriaID = categoriaId;
            EmpresaID = empresaId;
            Descripcion = descripcion;
            Color = color;
            Activo = activo;
        }

        private Categoria() { }
    }
}
