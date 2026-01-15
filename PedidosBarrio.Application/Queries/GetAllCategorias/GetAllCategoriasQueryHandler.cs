using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllCategorias
{
    public class GetAllCategoriasQueryHandler : IRequestHandler<GetAllCategoriasQuery, IEnumerable<CategoriaDto>>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetAllCategoriasQueryHandler(
            ICategoriaRepository categoriaRepository, 
            IMapper mapper, 
            ICurrentUserService currentUserService)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<CategoriaDto>> Handle(GetAllCategoriasQuery request, CancellationToken cancellationToken)
        {
            // Obtener EmpresaID del token
            var empresaId = _currentUserService.GetEmpresaId();

            var categorias = await _categoriaRepository.GetAllAsync(empresaId);
            return categorias.Select(c => new CategoriaDto
            {
                CategoriaID = c.CategoriaID,
                Descripcion = c.Descripcion,
                Color = c.Color,
                Activo = c.Activo
            });
        }
    }
}