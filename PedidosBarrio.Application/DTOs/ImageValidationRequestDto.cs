namespace PedidosBarrio.Application.DTOs
{
    public class ImageValidationRequestDto
    {
        /// <summary>
        /// URL de la imagen a validar
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Imagen en base64 (alternativa a ImageUrl)
        /// </summary>
        public string Base64Image { get; set; }

        /// <summary>
        /// Nivel de tolerancia (LOW, MEDIUM, HIGH)
        /// </summary>
        public string ToleranceLevel { get; set; } = "MEDIUM";
    }
}