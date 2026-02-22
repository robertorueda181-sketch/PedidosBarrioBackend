using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetDashboard
{
    public class GetDashboardQuery : IRequest<DashboardDto>
    {
        public Guid EmpresaID { get; set; }

        public GetDashboardQuery(Guid empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
