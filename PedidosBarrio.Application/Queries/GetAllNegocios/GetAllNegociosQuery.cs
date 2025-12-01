using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetAllNegocios
{
    public class GetAllNegociosQuery : IRequest<IEnumerable<NegocioDto>>
    {
    }
}
