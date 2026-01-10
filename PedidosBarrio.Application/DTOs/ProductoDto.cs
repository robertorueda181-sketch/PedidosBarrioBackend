namespace PedidosBarrio.Application.DTOs
{
    public class ProductoDto
    {
        public int ProductoID { get; set; }
        public Guid EmpresaID { get; set; }
        public short CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
