using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetNegocioById
{
    public class GetNegocioByIdQuery : IRequest<NegocioDto>
    {
        public int NegocioID { get; set; }

        public GetNegocioByIdQuery(int negocioID)
        {
            NegocioID = negocioID;
        }
    }
}
