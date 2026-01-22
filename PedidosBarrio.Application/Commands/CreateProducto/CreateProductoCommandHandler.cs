using FluentValidation;
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
        private readonly IPrecioRepository _precioRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;
        private readonly IValidator<CreateProductoDto> _validator;

        public CreateProductoCommandHandler(
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository,
            IPrecioRepository precioRepository,
            IImagenRepository imagenRepository,
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUserService,
            IApplicationLogger logger,
            IValidator<CreateProductoDto> validator)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _precioRepository = precioRepository;
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
            _currentUserService = currentUserService;
            _logger = logger;
            _validator = validator;
        }

        public async Task<ProductoDto> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ===== VALIDAR DTO CON FLUENTVALIDATION =====
                var createDto = new CreateProductoDto
                {
                    CategoriaID = request.CategoriaID,
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    Stock = request.Stock,
                    StockMinimo = request.StockMinimo,
                    Inventario = request.Inventario,
                    Precios = request.Precios,
                    ImagenUrl = request.ImagenUrl,
                    ImagenDescripcion = request.ImagenDescripcion
                };

                var validationResult = await _validator.ValidateAsync(createDto, cancellationToken);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }

                // Obtener empresa del usuario logueado
                var empresaId = _currentUserService.GetEmpresaId();

                // Verificar que la categoría pertenezca a la empresa
                var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaID);
                if (categoria == null)
                {
                    throw new ApplicationException("La categoría especificada no existe");
                }

                if (categoria.EmpresaID != empresaId)
                {
                    throw new ApplicationException("La categoría no pertenece a su empresa");
                }

                // Crear el producto
                var producto = new Producto(empresaId, request.Nombre, request.Descripcion)
                {
                    CategoriaID = request.CategoriaID,
                    Stock = request.Stock,
                    StockMinimo = request.StockMinimo,
                    Inventario = request.Inventario
                };

                var productoId = await _productoRepository.AddAsync(producto);

                // Crear los precios asociados
                var preciosCreados = new List<Precio>();
                if (request.Precios != null && request.Precios.Any())
                {
                    foreach (var p in request.Precios)
                    {
                        var precio = new Precio(
                            0, // IdPrecio
                            productoId,
                            empresaId,
                            p.Descripcion ?? "Precio base",
                            p.Modalidad ?? "GENERAL",
                            p.Precio,
                            p.EsPrincipal
                        );
                        await _precioRepository.AddAsync(precio);
                        preciosCreados.Add(precio);
                    }
                }
                else
                {
                    // Si no se envían precios, crear un precio por defecto
                    var precioDefault = new Precio(0, productoId, empresaId, "Precio por definir", "GENERAL", 0, true);
                    await _precioRepository.AddAsync(precioDefault);
                    preciosCreados.Add(precioDefault);
                }

                // Crear imagen inicial si se proporciona
                if (!string.IsNullOrEmpty(request.ImagenUrl))
                {
                    var imageUrl = request.ImagenUrl;

                    // Si es URL externa, optimizarla
                    if (imageUrl.StartsWith("http"))
                    {
                        try
                        {
                            imageUrl = await _imageProcessingService.OptimizeAndSaveImageFromUrlAsync(
                                imageUrl, 
                                productoId, 
                                empresaId);
                        }
                        catch
                        {
                            // Ignorar error y usar URL original o dejar vacío
                        }
                    }

                    var imagen = new Imagen(productoId, imageUrl, empresaId, request.ImagenDescripcion ?? "");
                    await _imagenRepository.AddAsync(imagen);
                }

                await _logger.LogInformationAsync(
                    $"Producto creado: ID={productoId}, Nombre={producto.Nombre}, EmpresaID={empresaId}, Precios={preciosCreados.Count}",
                    "CreateProductoCommand");

                // Obtener datos para la respuesta
                var precios = await _precioRepository.GetByProductoIdAsync(productoId);
                var imagenes = await _imagenRepository.GetByProductoIdAsync(productoId);

                    var dto = new ProductoDto
                    {
                        ProductoID = productoId,
                        EmpresaID = producto.EmpresaID ?? Guid.Empty,
                        CategoriaID = producto.CategoriaID ?? 0,
                        Nombre = producto.Nombre,
                        Descripcion = producto.Descripcion ?? string.Empty,
                        FechaRegistro = producto.FechaRegistro ?? DateTime.Now,
                        Stock = producto.Stock,
                        StockMinimo = producto.StockMinimo ?? 0,
                        Inventario = producto.Inventario,
                        CategoriaNombre = categoria.Descripcion,
                        CategoriaColor = categoria.Color ?? string.Empty,
                        Precios = precios.Select(p => new PrecioDto
                        {
                            IdPrecio = p.IdPrecio,
                            PrecioValor = p.PrecioValor,
                            ExternalId = p.ExternalId,
                            EmpresaID = p.EmpresaID,
                            FechaCreacion = p.FechaCreacion,
                            Activo = p.Activo,
                            EsPrincipal = p.EsPrincipal
                        }).ToList(),
                        PrecioActual = precios.FirstOrDefault(p => p.EsPrincipal)?.PrecioValor ?? precios.FirstOrDefault()?.PrecioValor,
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
                            Activa = i.Activa ?? false,
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
                    $"Error al crear producto: {ex.Message}",
                    ex,
                    "CreateProductoCommand");
                throw new ApplicationException($"Error al crear el producto: {ex.Message}", ex);
            }
        }
    }
}