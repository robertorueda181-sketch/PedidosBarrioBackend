using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetNegociosByEmpresa
{
    public class GetNegociosByEmpresaQuery : IRequest<IEnumerable<NegocioDto>>
    {
        public Guid EmpresaID { get; set; }

        public GetNegociosByEmpresaQuery(Guid empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
