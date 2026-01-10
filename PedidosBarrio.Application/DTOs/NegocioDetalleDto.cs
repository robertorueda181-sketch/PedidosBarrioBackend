namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// DTO que devuelve detalles de un negocio (empresa) con sus productos y categorías
    /// </summary>
    public class NegocioDetalleDto
    {
        public Guid EmpresaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        public List<CategoriaDetalleDto> Categorias { get; set; } = new List<CategoriaDetalleDto>();
        public List<ProductoDetalleDto> Productos { get; set; } = new List<ProductoDetalleDto>();
    }

    /// <summary>
    /// DTO para categorías con mostrar activo
    /// </summary>
    public class CategoriaDetalleDto
    {
        public short CategoriaID { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public bool Mostrar { get; set; }
    }

    /// <summary>
    /// DTO simple para producto con imagen
    /// </summary>
    public class ProductoDetalleDto
    {
        public int ProductoID { get; set; }
        public Guid EmpresaID { get; set; }
        public short CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string URLImagen { get; set; }
    }
}

