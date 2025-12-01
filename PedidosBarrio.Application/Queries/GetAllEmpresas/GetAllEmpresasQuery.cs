using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetAllEmpresas
{
    public class GetAllEmpresasQuery : IRequest<IEnumerable<EmpresaDto>>
    {
    }
}
