namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// DTO para la respuesta de optimización de imagen
    /// </summary>
    public class OptimizeImageResponseDto
    {
        /// <summary>
        /// URL completa de la imagen optimizada
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de imagen procesada
        /// </summary>
        public string TipoImagen { get; set; } = string.Empty;

        /// <summary>
        /// Dimensiones finales de la imagen (ancho x alto)
        /// </summary>
        public string Dimensiones { get; set; } = string.Empty;

        /// <summary>
        /// Formato final de la imagen (siempre WebP)
        /// </summary>
        public string Formato { get; set; } = "webp";

        /// <summary>
        /// Mensaje informativo sobre el procesamiento
        /// </summary>
        public string Mensaje { get; set; } = string.Empty;
    }
}
