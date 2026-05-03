namespace PedidosBarrio.Application.DTOs
{
    public class GetOnlyCategoriasDto
    {
        public List<CategoriaDto> Categorias { get; set; } = new List<CategoriaDto>();
        public string EmpresaID { get; set; } = string.Empty;
        public int TotalCategorias { get; set; }
        public DateTime FechaConsulta { get; set; }
    }
}
