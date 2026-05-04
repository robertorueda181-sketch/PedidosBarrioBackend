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
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;

        public GetAllProductosQueryHandler(
            IProductoRepository productoRepository,
            ICurrentUserService currentUserService,
            IApplicationLogger logger)
        {
            _productoRepository = productoRepository;
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
                        PrecioActual = 0,
                        ImagenPrincipal = p.Presentaciones.FirstOrDefault().Opciones.FirstOrDefault().Imagen
                    };


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
