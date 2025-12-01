using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetInmueblesByEmpresa
{
    public class GetInmueblesByEmpresaQuery : IRequest<IEnumerable<InmuebleDto>>
    {
        public int EmpresaID { get; set; }

        public GetInmueblesByEmpresaQuery(int empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
