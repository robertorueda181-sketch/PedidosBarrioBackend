using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetInmueblesByEmpresa
{
    public class GetInmueblesByEmpresaQuery : IRequest<IEnumerable<InmuebleDto>>
    {
        public Guid EmpresaID { get; set; }

        public GetInmueblesByEmpresaQuery(Guid empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
