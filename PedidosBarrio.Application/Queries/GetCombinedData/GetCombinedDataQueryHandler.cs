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
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;

        public GetCombinedDataQueryHandler(
            ICategoriaRepository categoriaRepository,
            IProductoRepository productoRepository,
            ICurrentUserService currentUserService,
            IApplicationLogger logger)
        {
            _categoriaRepository = categoriaRepository;
            _productoRepository = productoRepository;
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

                var productoDtos = productos.Select(p => new ProductoDto
                {
                    ProductoID = p.ProductoID,
                    EmpresaID = p.EmpresaID,
                    CategoriaID = p.CategoriaID,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    FechaCreacion = p.FechaCreacion,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    Imagen = p.Imagen,
                    CategoriaNombre = categoriaLookup.ContainsKey(p.CategoriaID) 
                        ? categoriaLookup[p.CategoriaID].Descripcion 
                        : "Sin categoría",
                    CategoriaColor = categoriaLookup.ContainsKey(p.CategoriaID) 
                        ? categoriaLookup[p.CategoriaID].Color 
                        : "#CCCCCC"
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