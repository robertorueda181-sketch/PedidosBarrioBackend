using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetImagenesByProducto
{
    public class GetImagenesByProductoQuery : IRequest<IEnumerable<ImagenDto>>
    {
        public int ProductoID { get; set; }

        public GetImagenesByProductoQuery(int productoID)
        {
            ProductoID = productoID;
        }
    }
}
