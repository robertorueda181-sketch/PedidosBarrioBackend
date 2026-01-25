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
        private readonly IPresentacionRepository _presentacionRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;
        private readonly IValidator<UpdateProductoDto> _validator;

        public UpdateProductoCommandHandler(
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository,
            IPrecioRepository precioRepository,
            IPresentacionRepository presentacionRepository,
            IImagenRepository imagenRepository,
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUserService,
            IApplicationLogger logger,
            IValidator<UpdateProductoDto> validator)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _precioRepository = precioRepository;
            _presentacionRepository = presentacionRepository;
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
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
                    Visible = request.Visible,
                    Precios = request.Precios
                };

                var validationResult = await _validator.ValidateAsync(updateDto, cancellationToken);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }

                // Obtener empresa del usuario logueado
                var empresaId = _currentUserService.GetEmpresaId();

                // Verificar que el producto existe y pertenece a la empresa
                var producto = await _productoRepository.GetByIdAsync(request.ProductoID, empresaId);
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

                // Actualizar producto usando Entity Framework
                producto.CategoriaID = request.CategoriaID;
                producto.Nombre = request.Nombre;
                producto.Descripcion = request.Descripcion;
                producto.Stock = request.Stock;
                producto.StockMinimo = request.StockMinimo;
                producto.Inventario = request.Inventario;
                producto.Visible = request.Visible;

                await _productoRepository.UpdateAsync(producto);

                // 1. Obtener todas las presentaciones y precios actuales del producto
                var presentacionesActuales = (await _presentacionRepository.GetByProductoIdAsync(producto.ProductoID)).ToList();
                var preciosActuales = presentacionesActuales.SelectMany(p => p.Precios).ToList();
                var idsPreciosIncoming = request.Precios.Where(p => p.IdPrecio > 0).Select(p => p.IdPrecio).ToList();

                // 2. Eliminar precios que ya no vienen en la solicitud
                foreach (var precioExistente in preciosActuales)
                {
                    if (!idsPreciosIncoming.Contains(precioExistente.IdPrecio))
                    {
                        await _precioRepository.DeleteAsync(precioExistente.IdPrecio);
                    }
                }

                // 3. Manejar lista de precios (Agregar o Actualizar)
                if (request.Precios != null && request.Precios.Any())
                {
                    foreach (var precioDto in request.Precios)
                    {
                        if (precioDto.IdPrecio == 0)
                        {
                            // Intentar reutilizar una presentación con la misma descripción para evitar duplicados
                            var descripcion = precioDto.Descripcion ?? "General";
                            var presentacionExistente = presentacionesActuales
                                .FirstOrDefault(p => p.Descripcion.Equals(descripcion, StringComparison.OrdinalIgnoreCase));

                            int presentacionId;
                            if (presentacionExistente == null)
                            {
                                var nuevaPresentacion = new Presentacion(descripcion, empresaId, producto.ProductoID);
                                presentacionId = await _presentacionRepository.AddAsync(nuevaPresentacion);
                                
                                // Agregar a la lista local para posible reutilización en el mismo bucle
                                nuevaPresentacion.PresentacionID = presentacionId;
                                presentacionesActuales.Add(nuevaPresentacion);
                            }
                            else
                            {
                                presentacionId = presentacionExistente.PresentacionID;
                            }

                            // Crear el nuevo precio vinculado a la presentación (existente o nueva)
                            var nuevoPrecio = new Precio(precioDto.PrecioValor, presentacionId, empresaId, precioDto.EsPrincipal, descripcion);
                            await _precioRepository.AddAsync(nuevoPrecio);
                        }
                        else
                        {
                            // Actualizar precio existente
                            var existingPrecio = await _precioRepository.GetByIdAsync(precioDto.IdPrecio);
                            if (existingPrecio != null && existingPrecio.EmpresaID == empresaId)
                            {
                                existingPrecio.PrecioValor = precioDto.PrecioValor;
                                existingPrecio.Principal = precioDto.EsPrincipal;
                                existingPrecio.Descripcion = precioDto.Descripcion;
                                await _precioRepository.UpdateAsync(existingPrecio);
                            }
                        }
                    }
                }

                // 4. Limpieza: Eliminar presentaciones que se hayan quedado sin precios
                // (Opcional, pero recomendado para mantener la BD limpia)
                var presentacionesDespues = await _presentacionRepository.GetByProductoIdAsync(producto.ProductoID);
                foreach (var pres in presentacionesDespues)
                {
                    if (!pres.Precios.Any())
                    {
                        await _presentacionRepository.DeleteAsync(pres.PresentacionID);
                    }
                }

                await _logger.LogInformationAsync(
                    $"Producto actualizado: ID={producto.ProductoID}, Nombre={producto.Nombre}, EmpresaID={empresaId}",
                    "UpdateProductoCommand");

                    // Obtener precios e imágenes para la respuesta
                    var precios = await _precioRepository.GetByProductoIdAsync(producto.ProductoID);
                    var imagenes = await _imagenRepository.GetByProductoIdAsync(producto.ProductoID);

                    var dto = new ProductoDto
                    {
                        ProductoID = producto.ProductoID,
                        CategoriaID = producto.CategoriaID ?? 0,
                        Nombre = producto.Nombre,
                        Descripcion = producto.Descripcion ?? string.Empty,
                        FechaRegistro = producto.FechaRegistro ?? DateTime.Now,
                        Stock = producto.Stock,
                        StockMinimo = producto.StockMinimo ?? 0,
                        Visible = producto.Visible ?? true,
                        Inventario = producto.Inventario,
                        PrecioActual = precios.FirstOrDefault(p => p.Principal)?.PrecioValor ?? precios.FirstOrDefault()?.PrecioValor,
                        Imagenes = new List<ImagenProductoDto>()
                    };

                    foreach (var i in imagenes)
                    {
                        var imgDto = new ImagenProductoDto
                        {
                            ImagenID = i.ImagenID,
                            ExternalId = i.ExternalId ?? 0,
                            URLImagen = i.URLImagen ?? string.Empty,
                            Descripcion = i.Descripcion ?? string.Empty,
                            FechaRegistro = i.FechaRegistro ?? DateTime.Now,
                            Activa = i.Activa,
                            Type = i.Type ?? string.Empty,
                            Order = i.Order,
                            EmpresaID = i.EmpresaID ?? Guid.Empty
                        };

                        if (!string.IsNullOrEmpty(imgDto.URLImagen))
                        {
                            imgDto.URLImagen = await _imageProcessingService.GetImageUrlAsync(imgDto.URLImagen);
                        }
                        dto.Imagenes.Add(imgDto);
                    }

                    dto.ImagenPrincipal = dto.Imagenes.OrderBy(i => i.Order).FirstOrDefault()?.URLImagen ?? string.Empty;

                    return dto;
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