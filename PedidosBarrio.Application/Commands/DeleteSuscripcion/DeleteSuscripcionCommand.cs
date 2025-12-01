using MediatR;

namespace PedidosBarrio.Application.Commands.DeleteSuscripcion
{
    public class DeleteSuscripcionCommand : IRequest<Unit>
    {
        public int SuscripcionID { get; set; }

        public DeleteSuscripcionCommand(int suscripcionID)
        {
            SuscripcionID = suscripcionID;
        }
    }
}
