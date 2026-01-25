using MediatR;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.DeleteProducto
{
    public class DeleteProductoCommandHandler : IRequestHandler<DeleteProductoCommand, Unit>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteProductoCommandHandler(IProductoRepository productoRepository, ICurrentUserService currentUserService)
        {
            _productoRepository = productoRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteProductoCommand command, CancellationToken cancellationToken)
        {
            var empresaId = _currentUserService.GetEmpresaId();
            var producto = await _productoRepository.GetByIdAsync(command.ProductoID, empresaId);
            
            if (producto == null)
            {
                throw new ApplicationException($"Producto with ID {command.ProductoID} not found.");
            }

            await _productoRepository.DeleteAsync(command.ProductoID);
            return Unit.Value;
        }
    }
}
