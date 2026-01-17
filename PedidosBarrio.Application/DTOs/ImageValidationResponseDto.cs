namespace PedidosBarrio.Application.DTOs
{
    public class ImageValidationResponseDto
    {
        /// <summary>
        /// Indica si la imagen es apropiada
        /// </summary>
        public bool IsAppropriate { get; set; }

        /// <summary>
        /// Nivel de confianza (0-1)
        /// </summary>
        public double ConfidenceScore { get; set; }

        /// <summary>
        /// Razones por las que puede ser inapropiada
        /// </summary>
        public List<string> ViolationReasons { get; set; } = new List<string>();

        /// <summary>
        /// Detalles de la detección
        /// </summary>
        public ImageModerationDetails Details { get; set; } = new ImageModerationDetails();

        /// <summary>
        /// Mensaje descriptivo
        /// </summary>
        public string Message { get; set; }
    }

    public class ImageModerationDetails
    {
        public bool HasAdultContent { get; set; }
        public bool HasViolentContent { get; set; }
        public bool HasMedicalContent { get; set; }
        public bool HasSpoofContent { get; set; }
        public bool HasRacyContent { get; set; }

        public string AdultLikelihood { get; set; }
        public string ViolenceLikelihood { get; set; }
        public string MedicalLikelihood { get; set; }
        public string SpoofLikelihood { get; set; }
        public string RacyLikelihood { get; set; }
    }
}