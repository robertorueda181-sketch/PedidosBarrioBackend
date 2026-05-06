using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetAllProductos
{
    public class GetAllProductosQuery : IRequest<GetAllProductosDto>
    {
        public string Codigo { get; set; }

        public GetAllProductosQuery(string codigo)
        {
            Codigo = codigo;
        }
    }
}
