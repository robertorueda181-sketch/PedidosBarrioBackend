namespace PedidosBarrio.Application.DTOs
{
    public class ProductoDto
    {
        public int ProductoID { get; set; }
        public Guid EmpresaID { get; set; }
        public int CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Imagen { get; set; }
        
        // Datos adicionales para respuesta completa
        public string CategoriaNombre { get; set; }
        public string CategoriaColor { get; set; }
    }
}
