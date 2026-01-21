namespace PedidosBarrio.Domain.Entities
{
    public class Precio
    {
        public int IdPrecio { get; set; }
        public decimal PrecioValor { get; set; }
        public int ExternalId { get; set; } // ProductoID
        public Guid EmpresaID { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }

        public bool EsPrincipal { get; set; } = false;

        // Navigation property
        public Producto Producto { get; set; }

        public Precio(decimal precioValor, int productoId, Guid empresaId, string descripcion = "", 
                     string modalidad = "GENERAL", int? cantidadMinima = null, bool esPrincipal = false)
        {
            PrecioValor = precioValor;
            ExternalId = productoId;
            EmpresaID = empresaId;
            EsPrincipal = esPrincipal;
            FechaCreacion = DateTime.UtcNow;
            Activo = true;
        }

        private Precio() { }
    }
}