using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateProducto
{
    public class CreateProductoCommandHandler : IRequestHandler<CreateProductoCommand, ProductoDto>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;

        public CreateProductoCommandHandler(
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

        public async Task<ProductoDto> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
        {
            // Obtener empresa del usuario logueado
            var empresaId = _currentUserService.GetEmpresaId();

            // Crear el producto
            var producto = new Producto(empresaId, request.Nombre, request.Descripcion)
            {
                CategoriaID = request.CategoriaID,
                Precio = request.Precio,
                Stock = request.Stock,
                Imagen = request.Imagen ?? string.Empty
            };

            await _productoRepository.AddAsync(producto);

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