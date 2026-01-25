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
        private readonly IPresentacionRepository _presentacionRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;
        private readonly IValidator<CreateProductoDto> _validator;

        public CreateProductoCommandHandler(
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository,
            IPrecioRepository precioRepository,
            IPresentacionRepository presentacionRepository,
            IImagenRepository imagenRepository,
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUserService,
            IApplicationLogger logger,
            IValidator<CreateProductoDto> validator)
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

                // Crear los precios asociados (vía Presentaciones)
                var preciosCreados = new List<Precio>();
                if (request.Precios != null && request.Precios.Any())
                {
                    foreach (var p in request.Precios)
                    {
                        // 1. Crear la presentación
                        var presentacion = new Presentacion(
                            p.Descripcion ?? "General",
                            empresaId,
                            productoId
                        );
                        var presentacionId = await _presentacionRepository.AddAsync(presentacion);

                        // 2. Crear el precio vinculado a la presentación
                        var precio = new Precio(
                            p.PrecioValor,
                            presentacionId,
                            empresaId,
                            p.EsPrincipal,
                            p.Descripcion
                        );

                        await _precioRepository.AddAsync(precio);
                        preciosCreados.Add(precio);
                    }
                }
                else
                {
                    // Si no se envían precios, crear una presentación y un precio por defecto
                    var presentacionDefault = new Presentacion("General", empresaId, productoId);
                    var presId = await _presentacionRepository.AddAsync(presentacionDefault);

                    var precioDefault = new Precio(0, presId, empresaId, true, "Precio por definir");
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
                            await _logger.LogInformationAsync(
                                $"Error de sanitizacion de imagen: ID={productoId}, Nombre={producto.Nombre}, EmpresaID={empresaId}, Precios={preciosCreados.Count}",
                                "CreateProductoCommand");
                        }
                    }

                    //var imagen = new Imagen(productoId, imageUrl, empresaId, request.ImagenDescripcion ?? "");
                    //await _imagenRepository.AddAsync(imagen);
                }

                await _logger.LogInformationAsync(
                    $"Producto creado: ID={productoId}, Nombre={producto.Nombre}, EmpresaID={empresaId}, Precios={preciosCreados.Count}",
                    "CreateProductoCommand");

               
               return new ProductoDto() { ProductoID = productoId };
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