using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetAllCategorias
{
    public class GetAllCategoriasQuery : IRequest<IEnumerable<CategoriaDto>>
    {
    }
}