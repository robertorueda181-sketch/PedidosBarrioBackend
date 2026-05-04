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
        private readonly IPresentacionRepository _presentacionRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IPasoInicialRepository _pasoInicialRepository;
        private readonly IApplicationLogger _logger;
        private readonly IValidator<CreateProductoDto> _validator;

        public CreateProductoCommandHandler(
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository,
            IPresentacionRepository presentacionRepository,
            IImagenRepository imagenRepository,
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUserService,
            IEmpresaRepository empresaRepository,
            IPasoInicialRepository pasoInicialRepository,
            IApplicationLogger logger,
            IValidator<CreateProductoDto> validator)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _presentacionRepository = presentacionRepository;
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
            _currentUserService = currentUserService;
            _empresaRepository = empresaRepository;
            _pasoInicialRepository = pasoInicialRepository;
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
                    Codigo = request.Codigo,
                    CategoriaDescripcion = request.CategoriaDescripcion,
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    Stock = request.Stock,
                    StockMinimo = request.StockMinimo,
                    Inventario = request.Inventario,
                    //Precios = request.Precios,
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

                // Resolver CategoriaID por descripción
                var categoriasEmpresa = await _categoriaRepository.GetByEmpresaIdAsync(empresaId);
                var categoria = categoriasEmpresa.FirstOrDefault(c => 
                    c.Descripcion.Trim().Equals(request.CategoriaDescripcion.Trim(), StringComparison.OrdinalIgnoreCase));
                
                if (categoria == null)
                {
                    throw new ApplicationException($"La categoría '{request.CategoriaDescripcion}' especificada no existe en su empresa");
                }

                // Verificar que la categoría pertenezca a la empresa
                if (categoria.EmpresaID != empresaId)
                {
                    throw new ApplicationException("La categoría no pertenece a su empresa");
                }

                // Crear el producto
                var producto = new Producto(empresaId, request.Nombre, request.Descripcion)
                {
                    Codigo = request.Codigo,
                    CategoriaID = categoria.CategoriaID,
                    Stock = request.Stock,
                    StockMinimo = request.StockMinimo,
                    Inventario = request.Inventario
                };

                var productoId = await _productoRepository.AddAsync(producto);

                //if (request.Precios != null && request.Precios.Any())
                //{
                //    foreach (var p in request.Precios)
                //    {
                //        // 1. Crear la presentación
                //        var presentacion = new Presentacion(
                //            p.Descripcion ?? "General",
                //            empresaId,
                //            productoId
                //        );
                //        var presentacionId = await _presentacionRepository.AddAsync(presentacion);
                //    }
                //}
                //else
                //{
                //    // Si no se envían precios, crear una presentación y un precio por defecto
                //    var presentacionDefault = new Presentacion("General", empresaId, productoId);
                //    var presId = await _presentacionRepository.AddAsync(presentacionDefault);

                //}

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
                                                $"Error de sanitizacion de imagen: ID={productoId}, Nombre={producto.Nombre}, EmpresaID={empresaId}",
                                                "CreateProductoCommand");
                                        }
                                    }

                                    //var imagen = new Imagen(productoId, imageUrl, empresaId, request.ImagenDescripcion ?? "");
                                    //await _imagenRepository.AddAsync(imagen);
                                }

                                await _logger.LogInformationAsync(
                                    $"Producto creado: ID={productoId}, Nombre={producto.Nombre}, EmpresaID={empresaId}",
                                    "CreateProductoCommand");

                                // ===== EVALUAR PASOS INICIALES =====
                                await EvaluarPasosInicialesAsync(empresaId);

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

                        /// <summary>
                        /// Evalúa si se deben marcar pasos iniciales como completados al agregar un producto
                        /// Si todos los pasos están completos, marca la empresa como visible y desactiva PasosIniciales
                        /// </summary>
                        private async Task EvaluarPasosInicialesAsync(Guid empresaId)
                        {
                            try
                            {
                                // Obtener PasosIniciales del token (sin query a BD)
                                var pasosIniciales = _currentUserService.GetPasosIniciales();
                                if (!pasosIniciales)
                                {
                                    // Si PasosIniciales es false, no evaluar
                                    return;
                                }

                                // Marcar "CREAR_PRODUCTO" como completado
                                var paso = await _pasoInicialRepository.GetPasoPorCodigoAsync(empresaId, "CREAR_PRODUCTO");
                                if (paso != null && !paso.Completado)
                                {
                                    await _pasoInicialRepository.CompletarPasoAsync(paso.PasoID);
                                    await _logger.LogInformationAsync(
                                        $"Paso CREAR_PRODUCTO marcado como completado para empresa {empresaId}",
                                        "CreateProductoCommand");
                                }

                                // Evaluar si TODOS los pasos iniciales están completos
                                await VerificarYFinalizarPasosInicialesAsync(empresaId);
                            }
                            catch (Exception ex)
                            {
                                // No fallar la creación del producto si hay error en los pasos iniciales
                                await _logger.LogWarningAsync(
                                    $"Error al evaluar pasos iniciales para empresa {empresaId}: {ex.Message}",
                                    "CreateProductoCommand");
                            }
                        }

                        /// <summary>
                        /// Verifica si todos los pasos iniciales están completados
                        /// Si es así, marca la empresa como visible y desactiva PasosIniciales
                        /// </summary>
                        private async Task VerificarYFinalizarPasosInicialesAsync(Guid empresaId)
                        {
                            try
                            {
                                // Obtener todos los pasos iniciales de la empresa
                                var todosLosPasos = await _pasoInicialRepository.GetPasosPorEmpresaAsync(empresaId);

                                if (todosLosPasos == null || !todosLosPasos.Any())
                                {
                                    return;
                                }

                                // Verificar si TODOS los pasos están completados
                                var todosCompletados = todosLosPasos.All(p => p.Completado);

                                if (todosCompletados)
                                {
                                    // Obtener la empresa
                                    var empresa = await _empresaRepository.GetByIdAsync(empresaId);
                                    if (empresa != null)
                                    {
                                        // Marcar como visible y desactivar evaluación de pasos iniciales
                                        empresa.Visible = true;
                                        empresa.PasosIniciales = false;

                                        await _empresaRepository.UpdateAsync(empresa);
                                        await _logger.LogInformationAsync(
                                            $"Empresa {empresaId} finalizada: Visible=true, PasosIniciales=false. Todos los pasos iniciales completados.",
                                            "CreateProductoCommand");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // No fallar si hay error al finalizar pasos
                                await _logger.LogWarningAsync(
                                    $"Error al verificar finalización de pasos para empresa {empresaId}: {ex.Message}",
                                    "CreateProductoCommand");
                            }
                        }
                    }
                }