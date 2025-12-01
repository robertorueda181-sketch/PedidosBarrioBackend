using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetSuscripcionById
{
    public class GetSuscripcionByIdQueryHandler : IRequestHandler<GetSuscripcionByIdQuery, SuscripcionDto>
    {
        private readonly ISuscripcionRepository _suscripcionRepository;
        private readonly IMapper _mapper;

        public GetSuscripcionByIdQueryHandler(ISuscripcionRepository suscripcionRepository, IMapper mapper)
        {
            _suscripcionRepository = suscripcionRepository;
            _mapper = mapper;
        }

        public async Task<SuscripcionDto> Handle(GetSuscripcionByIdQuery query, CancellationToken cancellationToken)
        {
            var suscripcion = await _suscripcionRepository.GetByIdAsync(query.SuscripcionID);
            if (suscripcion == null)
            {
                return null;
            }
            return _mapper.Map<SuscripcionDto>(suscripcion);
        }
    }
}
