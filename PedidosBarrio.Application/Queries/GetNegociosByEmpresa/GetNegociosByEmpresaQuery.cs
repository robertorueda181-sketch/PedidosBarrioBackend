using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetNegociosByEmpresa
{
    public class GetNegociosByEmpresaQuery : IRequest<IEnumerable<NegocioDto>>
    {
        public int EmpresaID { get; set; }

        public GetNegociosByEmpresaQuery(int empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
