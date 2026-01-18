using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetCombinedData
{
    public class GetCombinedDataQuery : IRequest<CombinedDataDto>
    {
        // Query sin parámetros porque usa el EmpresaId del usuario logueado
    }
}