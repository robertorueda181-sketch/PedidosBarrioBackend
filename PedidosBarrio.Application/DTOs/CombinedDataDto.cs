namespace PedidosBarrio.Application.DTOs
{
    public class CombinedDataDto
    {
        public List<CategoriaDto> Categorias { get; set; } = new List<CategoriaDto>();
        public List<ProductoDto> Productos { get; set; } = new List<ProductoDto>();
        public string EmpresaID { get; set; }
        public DateTime FechaConsulta { get; set; } = DateTime.UtcNow;
        public int TotalCategorias { get; set; }
        public int TotalProductos { get; set; }
    }
}