namespace PedidosBarrio.Application.DTOs
{
    public class GetAllProductosDto
    {
        public List<ProductoDto> Productos { get; set; } = new List<ProductoDto>();
        public string EmpresaID { get; set; }
        public int TotalProductos { get; set; }
        public DateTime FechaConsulta { get; set; }
    }
}
