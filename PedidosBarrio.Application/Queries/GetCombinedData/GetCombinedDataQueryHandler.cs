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
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;

        public GetCombinedDataQueryHandler(
            ICategoriaRepository categoriaRepository,
            IProductoRepository productoRepository,
            IPrecioRepository precioRepository,
            ICurrentUserService currentUserService,
            IApplicationLogger logger)
        {
            _categoriaRepository = categoriaRepository;
            _productoRepository = productoRepository;
            _precioRepository = precioRepository;
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

                var productoDtos = productos.Select(p => new ProductoDto
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
                }).ToList();

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