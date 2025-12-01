using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetAllImagenes
{
    public class GetAllImagenesQuery : IRequest<IEnumerable<ImagenDto>>
    {
    }
}
