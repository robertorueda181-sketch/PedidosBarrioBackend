using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetNegocioById
{
    public class GetNegocioByIdQuery : IRequest<NegocioDto>
    {
        public string NegocioID { get; set; }

        public GetNegocioByIdQuery(string negocioID)
        {
            NegocioID = negocioID;
        }
    }
}
