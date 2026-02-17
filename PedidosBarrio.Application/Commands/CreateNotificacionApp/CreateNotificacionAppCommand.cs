using MediatR;

namespace PedidosBarrio.Application.Commands.CreateNotificacionApp
{
    public class CreateNotificacionAppCommand : IRequest<int>
    {
        public string EmpresaCodigo { get; set; }
        public string Mensaje { get; set; }

        public CreateNotificacionAppCommand(string empresaCodigo, string mensaje)
        {
            EmpresaCodigo = empresaCodigo;
            Mensaje = mensaje;
        }
    }
}
