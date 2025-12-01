using MediatR;

namespace PedidosBarrio.Application.Commands.DeleteInmueble
{
    public class DeleteInmuebleCommand : IRequest<Unit>
    {
        public int InmuebleID { get; set; }

        public DeleteInmuebleCommand(int inmuebleID)
        {
            InmuebleID = inmuebleID;
        }
    }
}
