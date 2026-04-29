using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllProductos
{
    public class GetAllProductosQueryHandler : IRequestHandler<GetAllProductosQuery, GetAllProductosDto>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IPrecioRepository _precioRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;

        public GetAllProductosQueryHandler(
            IProductoRepository productoRepository,
            IPrecioRepository precioRepository,
            IImagenRepository imagenRepository,
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUserService,
            IApplicationLogger logger)
        {
            _productoRepository = productoRepository;
            _precioRepository = precioRepository;
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<GetAllProductosDto> Handle(GetAllProductosQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Obtener empresa del usuario logueado
                var empresaId = _currentUserService.GetEmpresaId();

                await _logger.LogInformationAsync(
                    $"Obteniendo todos los productos para empresa: {empresaId}",
                    "GetAllProductosQuery");

                // Obtener productos de la empresa
                var productos = await _productoRepository.GetByEmpresaIdAsync(empresaId);

                // Obtener precios para todos los productos
                var todosLosPrecios = await _precioRepository.GetByEmpresaIdAsync(empresaId);
                var preciosPorProducto = todosLosPrecios.GroupBy(p => p.Presentacion.ProductoID)
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
                        CategoriaID = p.CategoriaID ?? 0,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion ?? string.Empty,
                        FechaRegistro = p.FechaRegistro ?? DateTime.Now,
                        Stock = p.Stock,
                        StockMinimo = p.StockMinimo ?? 0,
                        Inventario = p.Inventario,
                        Visible = p.Visible ?? false,
                        Aprobado = p.Aprobado,
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

                var result = new GetAllProductosDto
                {
                    Productos = productoDtos,
                    EmpresaID = empresaId.ToString(),
                    TotalProductos = productoDtos.Count,
                    FechaConsulta = DateTime.UtcNow
                };

                await _logger.LogInformationAsync(
                    $"Se obtuvieron {result.TotalProductos} productos",
                    "GetAllProductosQuery");

                return result;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    $"Error al obtener productos: {ex.Message}",
                    ex,
                    "GetAllProductosQuery");
                throw new ApplicationException($"Error al obtener los productos: {ex.Message}", ex);
            }
        }
    }
}
