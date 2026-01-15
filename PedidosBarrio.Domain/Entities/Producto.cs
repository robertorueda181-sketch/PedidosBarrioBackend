namespace PedidosBarrio.Domain.Entities
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public Guid EmpresaID { get; set; }
        public int CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Imagen { get; set; }

        public Producto(Guid empresaID, string nombre, string descripcion)
        {
            EmpresaID = empresaID;
            Nombre = nombre;
            Descripcion = descripcion;
            FechaCreacion = DateTime.UtcNow;
        }

        private Producto() { }
    }
}
