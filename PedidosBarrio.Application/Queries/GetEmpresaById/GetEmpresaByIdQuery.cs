using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetEmpresaById
{
    public class GetEmpresaByIdQuery : IRequest<EmpresaDto>
    {
        public Guid EmpresaID { get; set; }

        public GetEmpresaByIdQuery(Guid empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
