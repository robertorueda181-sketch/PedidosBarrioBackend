using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateProducto
{
    public class UpdateProductoCommandHandler : IRequestHandler<UpdateProductoCommand, ProductoDto>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;

        public UpdateProductoCommandHandler(
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository,
            ICurrentUserService currentUserService,
            IApplicationLogger logger)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ProductoDto> Handle(UpdateProductoCommand request, CancellationToken cancellationToken)
        {
            // Obtener empresa del usuario logueado
            var empresaId = _currentUserService.GetEmpresaId();

            // Verificar que el producto existe y pertenece a la empresa
            var producto = await _productoRepository.GetByIdAsync(request.ProductoID);
            if (producto == null)
            {
                throw new ApplicationException("El producto especificado no existe");
            }

            if (producto.EmpresaID != empresaId)
            {
                throw new ApplicationException("El producto no pertenece a su empresa");
            }

            // Actualizar producto
            producto.CategoriaID = request.CategoriaID;
            producto.Nombre = request.Nombre;
            producto.Descripcion = request.Descripcion;
            producto.Precio = request.Precio;
            producto.Stock = request.Stock;
            producto.Imagen = request.Imagen ?? string.Empty;

            await _productoRepository.UpdateAsync(producto);

            // Obtener categoría para retornar información completa
            var categoria = await _categoriaRepository.GetByIdAsync((short)request.CategoriaID);

            return new ProductoDto
            {
                ProductoID = producto.ProductoID,
                EmpresaID = producto.EmpresaID,
                CategoriaID = producto.CategoriaID,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                FechaCreacion = producto.FechaCreacion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                Imagen = producto.Imagen,
                CategoriaNombre = categoria?.Descripcion ?? "Sin categoría",
                CategoriaColor = categoria?.Color ?? "#CCCCCC"
            };
        }
    }
}