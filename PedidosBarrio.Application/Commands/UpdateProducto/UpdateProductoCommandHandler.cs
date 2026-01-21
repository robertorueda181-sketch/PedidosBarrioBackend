using FluentValidation;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateProducto
{
    public class UpdateProductoCommandHandler : IRequestHandler<UpdateProductoCommand, ProductoDto>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IPrecioRepository _precioRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;
        private readonly IValidator<UpdateProductoDto> _validator;

        public UpdateProductoCommandHandler(
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository,
            IPrecioRepository precioRepository,
            ICurrentUserService currentUserService,
            IApplicationLogger logger,
            IValidator<UpdateProductoDto> validator)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _precioRepository = precioRepository;
            _currentUserService = currentUserService;
            _logger = logger;
            _validator = validator;
        }

        public async Task<ProductoDto> Handle(UpdateProductoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ===== VALIDAR DTO CON FLUENTVALIDATION =====
                var updateDto = new UpdateProductoDto
                {
                    CategoriaID = request.CategoriaID,
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    Stock = request.Stock,
                    StockMinimo = request.StockMinimo,
                    Inventario = request.Inventario,
                    NuevoPrecio = request.NuevoPrecio
                };

                var validationResult = await _validator.ValidateAsync(updateDto, cancellationToken);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }

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

                // Verificar que la nueva categoría pertenece a la empresa
                var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaID);
                if (categoria == null)
                {
                    throw new ApplicationException("La categoría especificada no existe");
                }

                if (categoria.EmpresaID != empresaId)
                {
                    throw new ApplicationException("La categoría no pertenece a su empresa");
                }

                // Actualizar producto
                producto.CategoriaID = request.CategoriaID;
                producto.Nombre = request.Nombre;
                producto.Descripcion = request.Descripcion;
                producto.Stock = request.Stock;
                producto.StockMinimo = request.StockMinimo;
                producto.Inventario = request.Inventario;

                await _productoRepository.UpdateAsync(producto);

                // Si se proporciona un nuevo precio, agregarlo
                if (request.NuevoPrecio.HasValue)
                {
                    var nuevoPrecio = new Precio(request.NuevoPrecio.Value, producto.ProductoID, empresaId);
                    await _precioRepository.AddAsync(nuevoPrecio);
                }

                await _logger.LogInformationAsync(
                    $"Producto actualizado: ID={producto.ProductoID}, Nombre={producto.Nombre}, EmpresaID={empresaId}",
                    "UpdateProductoCommand");

                // Obtener precios para la respuesta
                var precios = await _precioRepository.GetByProductoIdAsync(producto.ProductoID);
                var precioActual = await _precioRepository.GetPrecioActualByProductoIdAsync(producto.ProductoID);

                return new ProductoDto
                {
                    ProductoID = producto.ProductoID,
                    EmpresaID = producto.EmpresaID,
                    CategoriaID = producto.CategoriaID,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    FechaRegistro = producto.FechaRegistro,
                    Stock = producto.Stock,
                    StockMinimo = producto.StockMinimo,
                    Activa = producto.Activa,
                    Inventario = producto.Inventario,
                    CategoriaNombre = categoria.Descripcion,
                    CategoriaColor = categoria.Color,
                    Precios = precios.Select(p => new PrecioDto
                    {
                        IdPrecio = p.IdPrecio,
                        PrecioValor = p.PrecioValor,
                        ExternalId = p.ExternalId,
                        EmpresaID = p.EmpresaID,
                        FechaCreacion = p.FechaCreacion,
                        Activo = p.Activo
                    }).ToList(),
                    PrecioActual = precioActual?.PrecioValor
                };
            }
            catch (ValidationException)
            {
                throw; // Re-lanzar excepciones de validación sin modificar
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    $"Error al actualizar producto ID {request.ProductoID}: {ex.Message}",
                    ex,
                    "UpdateProductoCommand");
                throw new ApplicationException($"Error al actualizar el producto: {ex.Message}", ex);
            }
        }
    }
}