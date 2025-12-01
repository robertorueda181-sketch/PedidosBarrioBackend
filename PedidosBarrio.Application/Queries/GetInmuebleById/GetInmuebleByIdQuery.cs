using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetInmuebleById
{
    public class GetInmuebleByIdQuery : IRequest<InmuebleDto>
    {
        public int InmuebleID { get; set; }

        public GetInmuebleByIdQuery(int inmuebleID)
        {
            InmuebleID = inmuebleID;
        }
    }
}
