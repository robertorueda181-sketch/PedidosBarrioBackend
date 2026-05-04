namespace PedidosBarrio.Application.DTOs
{
    public class PresentacionOpcionDto
    {
        public int PresentacionOpcionID { get; set; }

        /// <summary>
        /// Valor de la opción (ej: "M", "Rojo", "Grande")
        /// </summary>
        public string Valor { get; set; } = null!;

        public int PresentacionID { get; set; }

        /// <summary>
        /// Precio específico de esta opción (si es diferente al precio principal)
        /// </summary>
        public decimal? Precio { get; set; }

        /// <summary>
        /// URL de imagen específica para esta opción
        /// </summary>
        public string? Imagen { get; set; }

        /// <summary>
        /// Descripción adicional de la opción
        /// </summary>
        public string? Descripcion { get; set; }

        public bool Activa { get; set; }

        /// <summary>
        /// Stock disponible para esta opción específica
        /// </summary>
        public int? Stock { get; set; }

        public bool EsPrincipal { get; set; }
    }
}
