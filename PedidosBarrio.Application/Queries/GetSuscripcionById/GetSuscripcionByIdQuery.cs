using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetSuscripcionById
{
    public class GetSuscripcionByIdQuery : IRequest<SuscripcionDto>
    {
        public int SuscripcionID { get; set; }

        public GetSuscripcionByIdQuery(int suscripcionID)
        {
            SuscripcionID = suscripcionID;
        }
    }
}
