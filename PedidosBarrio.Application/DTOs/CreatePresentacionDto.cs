namespace PedidosBarrio.Application.DTOs
{
    public class CreatePresentacionDto
    {
        /// <summary>
        /// Nombre de la presentación (ej: "Talla", "Color", "Tamaño")
        /// </summary>
        public string Descripcion { get; set; } = null!;

        /// <summary>
        /// ID del producto
        /// </summary>
        public int ProductoID { get; set; }

        /// <summary>
        /// Opciones de esta presentación que se crearán junto con ella
        /// </summary>
        public List<CreatePresentacionOpcionDto> Opciones { get; set; } = new List<CreatePresentacionOpcionDto>();
    }
}
