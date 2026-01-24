using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetAllTipos
{
    public class GetAllTiposQuery : IRequest<IEnumerable<TipoDto>>
    {
        public string Param { get; set; }
    }
}
