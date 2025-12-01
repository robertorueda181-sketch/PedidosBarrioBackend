namespace PedidosBarrio.Domain.Entities
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public int EmpresaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }

        public Producto(int empresaID, string nombre, string descripcion)
        {
            EmpresaID = empresaID;
            Nombre = nombre;
            Descripcion = descripcion;
            FechaCreacion = DateTime.UtcNow;
        }

        private Producto() { }
    }
}
