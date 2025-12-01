using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetSuscripcionesByEmpresa
{
    public class GetSuscripcionesByEmpresaQuery : IRequest<IEnumerable<SuscripcionDto>>
    {
        public int EmpresaID { get; set; }

        public GetSuscripcionesByEmpresaQuery(int empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
