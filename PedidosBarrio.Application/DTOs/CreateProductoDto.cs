namespace PedidosBarrio.Application.DTOs
{
    public class CreateProductoDto
    {
        public short CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
        public int? StockMinimo { get; set; }
        public bool Inventario { get; set; }

        // Lista de precios del producto
        public List<PrecioCreateDto> Precios { get; set; } = new List<PrecioCreateDto>();

        // Imagen inicial (opcional)
        public string ImagenUrl { get; set; }
        public string ImagenDescripcion { get; set; }
    }

    public class PrecioCreateDto
    {
        public decimal PrecioValor { get; set; }
        public string Descripcion { get; set; } = ""; // Ej: "Precio unitario", "Precio por mayor"
        public int? CantidadMinima { get; set; } // Cantidad mínima para aplicar este precio
        public string Modalidad { get; set; } = "GENERAL"; // GENERAL, DELIVERY, PICKUP, etc.
        public bool EsPrincipal { get; set; } = false; // Precio principal para mostrar
    }
}
