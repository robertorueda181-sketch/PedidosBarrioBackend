using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetSuscripcionesByEmpresa
{
    public class GetSuscripcionesByEmpresaQuery : IRequest<IEnumerable<SuscripcionDto>>
    {
        public Guid EmpresaID { get; set; }

        public GetSuscripcionesByEmpresaQuery(Guid empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
