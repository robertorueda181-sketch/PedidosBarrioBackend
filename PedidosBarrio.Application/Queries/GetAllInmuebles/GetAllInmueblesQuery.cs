using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetAllInmuebles
{
    public class GetAllInmueblesQuery : IRequest<IEnumerable<InmuebleDto>>
    {
    }
}
