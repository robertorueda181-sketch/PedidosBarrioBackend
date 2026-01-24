using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetCombinedData
{
    public class GetCombinedDataQueryHandler : IRequestHandler<GetCombinedDataQuery, CombinedDataDto>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IPrecioRepository _precioRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;

        public GetCombinedDataQueryHandler(
            ICategoriaRepository categoriaRepository,
            IProductoRepository productoRepository,
            IPrecioRepository precioRepository,
            IImagenRepository imagenRepository,
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUserService,
            IApplicationLogger logger)
        {
            _categoriaRepository = categoriaRepository;
            _productoRepository = productoRepository;
            _precioRepository = precioRepository;
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<CombinedDataDto> Handle(GetCombinedDataQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Obtener empresa del usuario logueado
                var empresaId = _currentUserService.GetEmpresaId();

                await _logger.LogInformationAsync(
                    $"Obteniendo datos combinados para empresa: {empresaId}",
                    "GetCombinedDataQuery");

                // Obtener categorías de la empresa
                var categorias = await _categoriaRepository.GetByEmpresaIdAsync(empresaId);
                var categoriaDtos = categorias.Select(c => new CategoriaDto
                {
                    CategoriaID = c.CategoriaID,
                    Descripcion = c.Descripcion,
                    Color = c.Color,
                    Activo = c.Activo
                }).ToList();

                // Obtener productos de la empresa
                var productos = await _productoRepository.GetByEmpresaIdAsync(empresaId);

                // Crear diccionario de categorías para lookup eficiente
                var categoriaLookup = categorias.ToDictionary(c => (int)c.CategoriaID);

                // Obtener precios para todos los productos
                var todosLosPrecios = await _precioRepository.GetByEmpresaIdAsync(empresaId);
                var preciosPorProducto = todosLosPrecios.GroupBy(p => p.ExternalId)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(p => p.IdPrecio).ToList());

                // Obtener imágenes para todos los productos
                var todasLasImagenes = await _imagenRepository.GetByEmpresaIdAsync(empresaId);
                var imagenesPorProducto = todasLasImagenes.GroupBy(i => i.ExternalId ?? 0)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var productoDtos = new List<ProductoDto>();
                foreach (var p in productos)
                {
                    var dto = new ProductoDto
                    {
                        ProductoID = p.ProductoID,
                        EmpresaID = p.EmpresaID ?? Guid.Empty,
                        CategoriaID = p.CategoriaID ?? 0,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion ?? string.Empty,
                        FechaRegistro = p.FechaRegistro ?? DateTime.Now,
                        Stock = p.Stock,
                        StockMinimo = p.StockMinimo ?? 0,
                        Inventario = p.Inventario,
                        Visible = p.Visible ?? false,
                        CategoriaNombre = (p.CategoriaID.HasValue && categoriaLookup.ContainsKey((int)p.CategoriaID.Value)) 
                            ? categoriaLookup[(int)p.CategoriaID.Value].Descripcion 
                            : "Sin categoría",
                        CategoriaColor = (p.CategoriaID.HasValue && categoriaLookup.ContainsKey((int)p.CategoriaID.Value)) 
                            ? categoriaLookup[(int)p.CategoriaID.Value].Color 
                            : "#CCCCCC",
                        Precios = preciosPorProducto.ContainsKey(p.ProductoID)
                            ? preciosPorProducto[p.ProductoID].Select(precio => new PrecioDto
                            {
                                IdPrecio = precio.IdPrecio,
                                PrecioValor = precio.PrecioValor,
                                ExternalId = precio.ExternalId,
                                EmpresaID = precio.EmpresaID,
                                FechaCreacion = precio.FechaCreacion,
                                Activo = precio.Activo
                            }).ToList()
                            : new List<PrecioDto>(),
                        PrecioActual = preciosPorProducto.ContainsKey(p.ProductoID) && preciosPorProducto[p.ProductoID].Any()
                            ? preciosPorProducto[p.ProductoID].First().PrecioValor
                            : null
                    };

                    // Mapear imágenes con URL completa
                    if (imagenesPorProducto.ContainsKey(p.ProductoID))
                    {
                        foreach (var img in imagenesPorProducto[p.ProductoID])
                        {
                            var imgDto = new ImagenProductoDto
                            {
                                ImagenID = img.ImagenID,
                                ExternalId = img.ExternalId ?? 0,
                                URLImagen = img.Urlimagen,
                                Descripcion = img.Descripcion ?? string.Empty,
                                FechaRegistro = img.FechaRegistro ?? DateTime.Now,
                                Activa = img.Activa,
                                Type = img.Type ?? "PRODUCT",
                                Order = img.Order,
                                EmpresaID = img.EmpresaID ?? Guid.Empty
                            };

                            // Resolver URL completa
                            if (!string.IsNullOrEmpty(imgDto.URLImagen))
                            {
                                imgDto.URLImagen = await _imageProcessingService.GetImageUrlAsync(imgDto.URLImagen);
                            }

                            dto.Imagenes.Add(imgDto);
                        }

                        // Establecer imagen principal para el DTO
                        var principal = dto.Imagenes.OrderBy(i => i.Order).FirstOrDefault();
                        if (principal != null)
                        {
                            dto.ImagenPrincipal = principal.URLImagen;
                        }
                    }

                    productoDtos.Add(dto);
                }

                var result = new CombinedDataDto
                {
                    Categorias = categoriaDtos,
                    Productos = productoDtos,
                    EmpresaID = empresaId.ToString(),
                    TotalCategorias = categoriaDtos.Count,
                    TotalProductos = productoDtos.Count,
                    FechaConsulta = DateTime.UtcNow
                };

                await _logger.LogInformationAsync(
                    $"Datos obtenidos: {result.TotalCategorias} categorías, {result.TotalProductos} productos",
                    "GetCombinedDataQuery");

                return result;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    $"Error al obtener datos combinados: {ex.Message}",
                    ex,
                    "GetCombinedDataQuery");
                throw new ApplicationException($"Error al obtener los datos: {ex.Message}", ex);
            }
        }
    }
}