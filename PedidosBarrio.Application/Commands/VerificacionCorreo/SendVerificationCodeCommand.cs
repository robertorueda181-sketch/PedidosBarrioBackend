using MediatR;

namespace PedidosBarrio.Application.Commands.VerificacionCorreo
{
    public class SendVerificationCodeCommand : IRequest<bool>
    {
        public string Correo { get; set; }

        public SendVerificationCodeCommand(string correo)
        {
            Correo = correo;
        }
    }
}
