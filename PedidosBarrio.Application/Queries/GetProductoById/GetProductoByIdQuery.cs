using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetProductoById
{
    public class GetProductoByIdQuery : IRequest<ProductoDto>
    {
        public int ProductoID { get; set; }

        public GetProductoByIdQuery(int productoID)
        {
            ProductoID = productoID;
        }
    }
}
