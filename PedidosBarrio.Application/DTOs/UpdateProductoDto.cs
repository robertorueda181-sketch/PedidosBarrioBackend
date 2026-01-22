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
            public bool Visible { get; set; }

            public List<PrecioDto> Precios { get; set; } = new List<PrecioDto>();
        }
    }
