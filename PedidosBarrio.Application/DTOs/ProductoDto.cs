namespace PedidosBarrio.Application.DTOs
{
    public class ProductoDto
    {
        public int ProductoID { get; set; }
        public short CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int Stock { get; set; }
        public int? StockMinimo { get; set; }
        public bool? Activa { get; set; }
        public bool Inventario { get; set; }
        public bool Visible { get; set; }
        public bool Aprobado { get; set; }

        // Lista de presentaciones del producto (incluye sus precios)
        public List<PresentacionDto> Presentaciones { get; set; } = new List<PresentacionDto>();


        // Precio actual (el más reciente o principal)
        public decimal? PrecioActual { get; set; }


        // Imagen principal (primera o por order)
        public string ImagenPrincipal { get; set; }
    }

    public class PresentacionDto
    {
        public int PresentacionID { get; set; }
        public string Descripcion { get; set; }
        public int ProductoID { get; set; }
        public List<PresentacionOpcionDto> Opciones { get; set; } = new List<PresentacionOpcionDto>();
    } 
    public class ImagenProductoDto
    {
        public int ImagenID { get; set; }
        public int ExternalId { get; set; }
        public string URLImagen { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activa { get; set; }
        public string Type { get; set; }
        public short Order { get; set; }
        public Guid EmpresaID { get; set; }
    }

    public class UpdateProductoVisibleDto
    {
        public int ProductoID { get; set; }
        public bool Visible { get; set; }
    }
}

