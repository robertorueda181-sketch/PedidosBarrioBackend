using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Services
{
    public interface IImageModerationService
    {
        Task<ImageValidationResponseDto> ValidateImageAsync(string imageUrl, string toleranceLevel = "MEDIUM");
        Task<ImageValidationResponseDto> ValidateImageFromBase64Async(string base64Image, string toleranceLevel = "MEDIUM");
    }
}