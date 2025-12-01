using MediatR;

namespace PedidosBarrio.Application.Commands.DeleteProducto
{
    public class DeleteProductoCommand : IRequest<Unit>
    {
        public int ProductoID { get; set; }

        public DeleteProductoCommand(int productoID)
        {
            ProductoID = productoID;
        }
    }
}
