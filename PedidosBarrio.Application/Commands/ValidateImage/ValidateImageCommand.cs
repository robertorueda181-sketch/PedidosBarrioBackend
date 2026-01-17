using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.ValidateImage
{
    public class ValidateImageCommand : IRequest<ImageValidationResponseDto>
    {
        public string ImageUrl { get; set; }
        public string Base64Image { get; set; }
        public string ToleranceLevel { get; set; }

        public ValidateImageCommand(string imageUrl = null, string base64Image = null, string toleranceLevel = "MEDIUM")
        {
            ImageUrl = imageUrl;
            Base64Image = base64Image;
            ToleranceLevel = toleranceLevel;
        }
    }
}