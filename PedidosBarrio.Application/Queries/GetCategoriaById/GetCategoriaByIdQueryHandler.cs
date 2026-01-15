using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetCategoriaById
{
    public class GetCategoriaByIdQueryHandler : IRequestHandler<GetCategoriaByIdQuery, CategoriaDto>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;

        public GetCategoriaByIdQueryHandler(ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }

        public async Task<CategoriaDto> Handle(GetCategoriaByIdQuery request, CancellationToken cancellationToken)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId);
            
            if (categoria == null)
                return null;

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