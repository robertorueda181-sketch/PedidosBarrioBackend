namespace PedidosBarrio.Application.DTOs
{
    public class TextModerationResponseDto
    {
        /// <summary>
        /// Indica si el texto es apropiado
        /// </summary>
        public bool IsAppropriate { get; set; }

        /// <summary>
        /// Indica si se detectó contenido inapropiado
        /// </summary>
        public bool Flagged { get; set; }

        /// <summary>
        /// Categorías de violación detectadas
        /// </summary>
        public List<string> ViolationCategories { get; set; } = new List<string>();

        /// <summary>
        /// Detalles específicos de la moderación
        /// </summary>
        public TextModerationDetails Details { get; set; } = new TextModerationDetails();

        /// <summary>
        /// Mensaje descriptivo
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Score más alto de todas las categorías
        /// </summary>
        public double HighestScore { get; set; }
    }

    public class TextModerationDetails
    {
        // Categorías de moderación de OpenAI
        public bool Sexual { get; set; }
        public bool Hate { get; set; }
        public bool Harassment { get; set; }
        public bool SelfHarm { get; set; }
        public bool SexualMinors { get; set; }
        public bool HateThreatening { get; set; }
        public bool ViolenceGraphic { get; set; }
        public bool SelfHarmIntent { get; set; }
        public bool SelfHarmInstructions { get; set; }
        public bool HarassmentThreatening { get; set; }
        public bool Violence { get; set; }

        // Scores de confianza (0.0 - 1.0)
        public double SexualScore { get; set; }
        public double HateScore { get; set; }
        public double HarassmentScore { get; set; }
        public double SelfHarmScore { get; set; }
        public double SexualMinorsScore { get; set; }
        public double HateThreateningScore { get; set; }
        public double ViolenceGraphicScore { get; set; }
        public double SelfHarmIntentScore { get; set; }
        public double SelfHarmInstructionsScore { get; set; }
        public double HarassmentThreateningScore { get; set; }
        public double ViolenceScore { get; set; }
    }
}