using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetProductosByEmpresa
{
    public class GetProductosByEmpresaQuery : IRequest<IEnumerable<ProductoDto>>
    {
        public int EmpresaID { get; set; }

        public GetProductosByEmpresaQuery(int empresaID)
        {
            EmpresaID = empresaID;
        }
    }
}
