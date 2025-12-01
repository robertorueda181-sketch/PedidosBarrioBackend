using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetAllProductos
{
    public class GetAllProductosQuery : IRequest<IEnumerable<ProductoDto>>
    {
    }
}
