using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetTipos
{
    public class GetTiposQueryHandler : IRequestHandler<GetTiposQuery, IEnumerable<TipoDto>>
    {
        private readonly ITipoRepository _tipoRepository;
        private readonly IMapper _mapper;

        public GetTiposQueryHandler(ITipoRepository tipoRepository, IMapper mapper)
        {
            _tipoRepository = tipoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TipoDto>> Handle(GetTiposQuery request, CancellationToken cancellationToken)
        {
            var tipos = await _tipoRepository.GetTiposPorParametroAsync();
            // Mapeamos la columna "Tipo" devuelta por la función a TipoDto.Descripcion
            return tipos.Select(t => new TipoDto { Descripcion = t.Descripcion,Icono = t.Icono }).ToList();
        }
    }
}
