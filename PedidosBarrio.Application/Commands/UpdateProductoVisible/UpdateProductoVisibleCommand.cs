using MediatR;

namespace PedidosBarrio.Application.Commands.UpdateProductoVisible
{
    public class UpdateProductoVisibleCommand : IRequest<bool>
    {
        public int ProductoID { get; set; }
        public bool Visible { get; set; }

        public UpdateProductoVisibleCommand(int productoId, bool visible)
        {
            ProductoID = productoId;
            Visible = visible;
        }
    }
}
