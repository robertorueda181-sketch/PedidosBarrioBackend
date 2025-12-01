using MediatR;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.DeleteProducto
{
    public class DeleteProductoCommandHandler : IRequestHandler<DeleteProductoCommand, Unit>
    {
        private readonly IProductoRepository _productoRepository;

        public DeleteProductoCommandHandler(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<Unit> Handle(DeleteProductoCommand command, CancellationToken cancellationToken)
        {
            var producto = await _productoRepository.GetByIdAsync(command.ProductoID);
            if (producto == null)
            {
                throw new ApplicationException($"Producto with ID {command.ProductoID} not found.");
            }

            await _productoRepository.DeleteAsync(command.ProductoID);
            return Unit.Value;
        }
    }
}
