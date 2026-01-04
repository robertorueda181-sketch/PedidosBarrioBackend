using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetAllInmuebles
{
    /// <summary>
    /// Query para obtener TODOS los inmuebles CON detalles completos
    /// Incluye: Tipo de inmueble, Tipo de operación, Primera imagen
    /// </summary>
    public class GetAllInmueblesDetailsQuery : IRequest<IEnumerable<InmuebleDetailsDto>>
    {
    }
}
