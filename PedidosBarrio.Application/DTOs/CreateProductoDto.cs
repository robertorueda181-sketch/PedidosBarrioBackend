namespace PedidosBarrio.Application.DTOs
{
    public class CreateProductoDto
    {
        public Guid EmpresaID { get; set; }
        public short CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}
