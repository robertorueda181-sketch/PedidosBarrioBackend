using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetPaginaByCodigo
{
    public class GetPaginaByCodigoQuery : IRequest<PaginaDto>
    {
        public string Codigo { get; set; }

        public GetPaginaByCodigoQuery(string codigo)
        {
            Codigo = codigo;
        }
    }
}
