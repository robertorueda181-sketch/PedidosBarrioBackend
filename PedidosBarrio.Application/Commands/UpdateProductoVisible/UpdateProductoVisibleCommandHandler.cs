using MediatR;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateProductoVisible
{
    public class UpdateProductoVisibleCommandHandler : IRequestHandler<UpdateProductoVisibleCommand, bool>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateProductoVisibleCommandHandler(
            IProductoRepository productoRepository,
            ICurrentUserService currentUserService)
        {
            _productoRepository = productoRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateProductoVisibleCommand request, CancellationToken cancellationToken)
        {
            // Obtener empresa del usuario logueado para seguridad
            var empresaId = _currentUserService.GetEmpresaId();
            
            // Buscar el producto
            var producto = await _productoRepository.GetByIdAsync(request.ProductoID, empresaId);
            
            if (producto == null)
            {
                return false;
            }

            // Verificar propiedad
            if (producto.EmpresaID != empresaId)
            {
                throw new UnauthorizedAccessException("No tiene permisos para modificar este producto");
            }

            // Actualizar visibilidad
            producto.Visible = request.Visible;
            
            await _productoRepository.UpdateAsync(producto);
            
            return true;
        }
    }
}
