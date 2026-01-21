namespace PedidosBarrio.Application.DTOs
{
    public class UpdateProductoDto
    {
        public short CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
        public int? StockMinimo { get; set; }
        public bool Inventario { get; set; }

        // Nuevo precio (opcional - si se envía, se agrega como nuevo precio)
        public decimal? NuevoPrecio { get; set; }
    }
}