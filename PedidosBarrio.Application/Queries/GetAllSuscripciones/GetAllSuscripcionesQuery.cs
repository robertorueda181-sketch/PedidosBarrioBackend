using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetAllSuscripciones
{
    public class GetAllSuscripcionesQuery : IRequest<IEnumerable<SuscripcionDto>>
    {
    }
}
