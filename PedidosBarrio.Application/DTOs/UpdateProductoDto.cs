namespace PedidosBarrio.Application.DTOs
{
        public class UpdateProductoDto
        {
            public string Codigo { get; set; }
            public string CategoriaDescripcion { get; set; } // Descripcion de la categoria (se busca en la tabla Categoria)
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public int Stock { get; set; }
            public int? StockMinimo { get; set; }
            public bool Inventario { get; set; }
            public bool Visible { get; set; }

            public List<PrecioDto> Precios { get; set; } = new List<PrecioDto>();
        }
    }
