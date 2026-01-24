using MediatR;

namespace PedidosBarrio.Application.Commands.VerificacionCorreo
{
    public class VerifyCodeCommand : IRequest<bool>
    {
        public string Correo { get; set; }
        public string Codigo { get; set; }

        public VerifyCodeCommand(string correo, string codigo)
        {
            Correo = correo;
            Codigo = codigo;
        }
    }
}
