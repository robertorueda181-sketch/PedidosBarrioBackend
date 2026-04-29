using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetOnlyCategorias
{
    public class GetOnlyCategoriasQueryHandler : IRequestHandler<GetOnlyCategoriasQuery, GetOnlyCategoriasDto>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;

        public GetOnlyCategoriasQueryHandler(
            ICategoriaRepository categoriaRepository,
            ICurrentUserService currentUserService,
            IApplicationLogger logger)
        {
            _categoriaRepository = categoriaRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<GetOnlyCategoriasDto> Handle(GetOnlyCategoriasQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Obtener empresa del usuario logueado
                var empresaId = _currentUserService.GetEmpresaId();

                await _logger.LogInformationAsync(
                    $"Obteniendo categorías para empresa: {empresaId}",
                    "GetOnlyCategoriasQuery");

                // Obtener categorías de la empresa
                var categorias = await _categoriaRepository.GetByEmpresaIdAsync(empresaId);
                var categoriaDtos = categorias.Select(c => new CategoriaDto
                {
                    CategoriaID = c.CategoriaID,
                    Descripcion = c.Descripcion,
                    Color = c.Color,
                    Activo = c.Activo
                }).ToList();

                var result = new GetOnlyCategoriasDto
                {
                    Categorias = categoriaDtos,
                    TotalCategorias = categoriaDtos.Count,
                    FechaConsulta = DateTime.UtcNow
                };

                await _logger.LogInformationAsync(
                    $"Se obtuvieron {result.TotalCategorias} categorías",
                    "GetOnlyCategoriasQuery");

                return result;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    $"Error al obtener categorías: {ex.Message}",
                    ex,
                    "GetOnlyCategoriasQuery");
                throw new ApplicationException($"Error al obtener las categorías: {ex.Message}", ex);
            }
        }
    }
}
