using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.ModerateText
{
    public class ModerateTextCommand : IRequest<TextModerationResponseDto>
    {
        public string Text { get; set; }
        public string Model { get; set; }

        public ModerateTextCommand(string text, string model = "omni-moderation-latest")
        {
            Text = text;
            Model = model;
        }
    }
}