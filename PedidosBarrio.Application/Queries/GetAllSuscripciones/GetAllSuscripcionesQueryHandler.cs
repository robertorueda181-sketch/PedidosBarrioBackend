using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllSuscripciones
{
    public class GetAllSuscripcionesQueryHandler : IRequestHandler<GetAllSuscripcionesQuery, IEnumerable<SuscripcionDto>>
    {
        private readonly ISuscripcionRepository _suscripcionRepository;
        private readonly IMapper _mapper;

        public GetAllSuscripcionesQueryHandler(ISuscripcionRepository suscripcionRepository, IMapper mapper)
        {
            _suscripcionRepository = suscripcionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SuscripcionDto>> Handle(GetAllSuscripcionesQuery query, CancellationToken cancellationToken)
        {
            var suscripciones = await _suscripcionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SuscripcionDto>>(suscripciones);
        }
    }
}
