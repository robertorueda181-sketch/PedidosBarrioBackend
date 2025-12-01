using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllTipos
{
    public class GetAllTiposQueryHandler : IRequestHandler<GetAllTiposQuery, IEnumerable<TipoDto>>
    {
        private readonly ITipoRepository _tipoRepository;
        private readonly IMapper _mapper;

        public GetAllTiposQueryHandler(ITipoRepository tipoRepository, IMapper mapper)
        {
            _tipoRepository = tipoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TipoDto>> Handle(GetAllTiposQuery query, CancellationToken cancellationToken)
        {
            var tipos = await _tipoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TipoDto>>(tipos);
        }
    }
}
