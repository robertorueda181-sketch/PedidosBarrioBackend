namespace PedidosBarrio.Application.DTOs
{
    public class TextModerationRequestDto
    {
        /// <summary>
        /// Texto a moderar
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Modelo de moderación a usar (omni-moderation-latest, text-moderation-stable)
        /// </summary>
        public string Model { get; set; } = "omni-moderation-latest";
    }
}