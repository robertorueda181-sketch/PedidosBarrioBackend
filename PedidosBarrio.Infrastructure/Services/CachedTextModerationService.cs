using Microsoft.Extensions.Caching.Memory;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using System.Security.Cryptography;
using System.Text;

namespace PedidosBarrio.Infrastructure.Services
{
    public class CachedTextModerationService : ITextModerationService
    {
        private readonly ITextModerationService _baseService;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration;

        public CachedTextModerationService(
            OpenAITextModerationService baseService, 
            IMemoryCache cache)
        {
            _baseService = baseService;
            _cache = cache;
            _cacheDuration = TimeSpan.FromMinutes(10); // Cache por 10 minutos
        }

        public async Task<TextModerationResponseDto> ModerateTextAsync(string text, string model = "omni-moderation-latest")
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new TextModerationResponseDto
                {
                    IsAppropriate = true,
                    Flagged = false,
                    Message = "Texto vacío - considerado apropiado"
                };
            }


            // Si no está en cache, hacer el request
            var result = await _baseService.ModerateTextAsync(text, model);
            return result;
        }

       
    }
}