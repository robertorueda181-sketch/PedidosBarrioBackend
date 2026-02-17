using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetEmpresaSedeDetalle
{
    public class GetEmpresaSedeDetalleQuery : IRequest<EmpresaSedeDetalleDto>
    {
        public Guid EmpresaID { get; set; }

        public GetEmpresaSedeDetalleQuery(Guid empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
