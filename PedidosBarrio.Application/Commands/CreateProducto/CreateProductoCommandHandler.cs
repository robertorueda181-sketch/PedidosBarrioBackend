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
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;
        private readonly IValidator<CreateProductoDto> _validator;

        public CreateProductoCommandHandler(
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository,
            IPrecioRepository precioRepository,
            IImagenRepository imagenRepository,
            ICurrentUserService currentUserService,
            IApplicationLogger logger,
            IValidator<CreateProductoDto> validator)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _precioRepository = precioRepository;
            _imagenRepository = imagenRepository;
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

                // ===== CREAR LISTA DE PRECIOS =====
                var preciosCreados = new List<Precio>();

                if (request.Precios != null && request.Precios.Any())
                {
                    // Validar que solo haya un precio principal
                    var preciosPrincipales = request.Precios.Count(p => p.EsPrincipal);
                    if (preciosPrincipales > 1)
                    {
                        throw new ApplicationException("Solo puede haber un precio marcado como principal");
                    }

                    // Si no hay precio principal, marcar el primero como principal
                    if (preciosPrincipales == 0 && request.Precios.Any())
                    {
                        request.Precios.First().EsPrincipal = true;
                    }

                    // Crear cada precio
                    foreach (var precioDto in request.Precios)
                    {
                        var precio = new Precio(
                            precioDto.Precio, 
                            productoId, 
                            empresaId,
                            precioDto.Descripcion ?? "",
                            precioDto.Modalidad ?? "GENERAL",
                            precioDto.CantidadMinima,
                            precioDto.EsPrincipal);

                        await _precioRepository.AddAsync(precio);
                        preciosCreados.Add(precio);
                    }
                }
                else
                {
                    // Si no se envían precios, crear un precio por defecto
                    var precioDefault = new Precio(0, productoId, empresaId, "Precio por definir", "GENERAL", null, true);
                    await _precioRepository.AddAsync(precioDefault);
                    preciosCreados.Add(precioDefault);
                }

                // Crear imagen inicial si se proporciona
                if (!string.IsNullOrEmpty(request.ImagenUrl))
                {
                    var imagen = new Imagen(productoId, request.ImagenUrl, empresaId, request.ImagenDescripcion ?? "");
                    await _imagenRepository.AddAsync(imagen);
                }

                await _logger.LogInformationAsync(
                    $"Producto creado: ID={productoId}, Nombre={producto.Nombre}, EmpresaID={empresaId}, Precios={preciosCreados.Count}",
                    "CreateProductoCommand");

                // Obtener datos para la respuesta
                var precios = await _precioRepository.GetByProductoIdAsync(productoId);
                var imagenes = await _imagenRepository.GetByProductoIdAsync(productoId);

                return new ProductoDto
                {
                    ProductoID = productoId,
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
                        Activo = p.Activo,
                        EsPrincipal = p.EsPrincipal
                    }).ToList(),
                    PrecioActual = precios.FirstOrDefault(p => p.EsPrincipal)?.PrecioValor ?? precios.FirstOrDefault()?.PrecioValor,
                    Imagenes = imagenes.Select(i => new ImagenProductoDto
                    {
                        ImagenID = i.ImagenID,
                        ExternalId = i.ExternalId,
                        URLImagen = i.URLImagen,
                        Descripcion = i.Descripcion,
                        FechaRegistro = i.FechaRegistro,
                        Activa = i.Activa,
                        Type = i.Type,
                        Order = i.Order,
                        EmpresaID = i.EmpresaID
                    }).ToList(),
                    ImagenPrincipal = imagenes.FirstOrDefault()?.URLImagen
                };
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