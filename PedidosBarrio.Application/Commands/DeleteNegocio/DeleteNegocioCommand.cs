using MediatR;

namespace PedidosBarrio.Application.Commands.DeleteNegocio
{
    public class DeleteNegocioCommand : IRequest<Unit>
    {
        public int NegocioID { get; set; }

        public DeleteNegocioCommand(int negocioID)
        {
            NegocioID = negocioID;
        }
    }
}
