using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetCategoriaById
{
    public class GetCategoriaByIdQueryHandler : IRequestHandler<GetCategoriaByIdQuery, CategoriaDto>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetCategoriaByIdQueryHandler(
            ICategoriaRepository categoriaRepository, 
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<CategoriaDto> Handle(GetCategoriaByIdQuery request, CancellationToken cancellationToken)
        {
            // Obtener EmpresaID del token del usuario logueado
            var empresaIdUsuario = _currentUserService.GetEmpresaId();

            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId);
            
            if (categoria == null)
                return null;

            // Validar que la categoría pertenece a la empresa del usuario logueado
            if (categoria.EmpresaID != empresaIdUsuario)
            {
                throw new UnauthorizedAccessException("No tienes permisos para ver esta categoría. Solo puedes ver categorías de tu empresa.");
            }

            return new CategoriaDto
            {
                CategoriaID = categoria.CategoriaID,
                Descripcion = categoria.Descripcion,
                Color = categoria.Color,
                Activo = categoria.Activo
            };
        }
    }
}