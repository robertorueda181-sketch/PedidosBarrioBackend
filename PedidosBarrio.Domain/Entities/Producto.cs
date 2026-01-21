namespace PedidosBarrio.Domain.Entities
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public Guid EmpresaID { get; set; }
        public short CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activa { get; set; }
        public bool Visible { get; set; }
        public int? StockMinimo { get; set; }
        public bool Inventario { get; set; }
        public string Imagen { get; set; }



        // Navigation property para precios
        public List<Precio> Precios { get; set; } = new List<Precio>();

        public Producto(Guid empresaID, string nombre, string descripcion)
        {
            EmpresaID = empresaID;
            Nombre = nombre;
            Descripcion = descripcion;
            FechaRegistro = DateTime.UtcNow;
            Activa = true;
            Stock = 0;
            Inventario = false;
        }

        private Producto() { }
    }
}
