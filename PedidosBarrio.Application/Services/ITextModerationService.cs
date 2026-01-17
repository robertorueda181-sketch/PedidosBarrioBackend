using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Services
{
    public interface ITextModerationService
    {
        Task<TextModerationResponseDto> ModerateTextAsync(string text, string model = "text-moderation-latest");
    }
}