namespace PedidosBarrio.Application.DTOs
{
    public class PresentacionDetalleDto
    {
        public int PresentacionID { get; set; }

        /// <summary>
        /// Nombre de la presentación (ej: "Talla", "Color", "Tamaño")
        /// </summary>
        public string Descripcion { get; set; } = null!;

        public int ProductoID { get; set; }

        public bool Activa { get; set; }

        /// <summary>
        /// Opciones disponibles para esta presentación
        /// </summary>
        public List<PresentacionOpcionDto> Opciones { get; set; } = new List<PresentacionOpcionDto>();
    }
}
