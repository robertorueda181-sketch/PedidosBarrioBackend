using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetProductosByEmpresa
{
    public class GetProductosByEmpresaQuery : IRequest<IEnumerable<ProductoDto>>
    {
        public Guid EmpresaID { get; set; }

        public GetProductosByEmpresaQuery(Guid empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
