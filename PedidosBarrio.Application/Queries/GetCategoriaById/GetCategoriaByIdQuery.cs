using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetCategoriaById
{
    public class GetCategoriaByIdQuery : IRequest<CategoriaDto>
    {
        public short CategoriaId { get; set; }

        public GetCategoriaByIdQuery(short categoriaId)
        {
            CategoriaId = categoriaId;
        }
    }
}