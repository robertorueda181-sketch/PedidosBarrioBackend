using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetTipos
{
    public class GetTiposQuery : IRequest<IEnumerable<TipoDto>>
    {
    }
}
