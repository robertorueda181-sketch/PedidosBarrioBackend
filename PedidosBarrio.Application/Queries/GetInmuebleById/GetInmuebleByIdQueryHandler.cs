using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetInmuebleById
{
    public class GetInmuebleByIdQueryHandler : IRequestHandler<GetInmuebleByIdQuery, InmuebleDto>
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IMapper _mapper;

        public GetInmuebleByIdQueryHandler(IInmuebleRepository inmuebleRepository, IMapper mapper)
        {
            _inmuebleRepository = inmuebleRepository;
            _mapper = mapper;
        }

        public async Task<InmuebleDto> Handle(GetInmuebleByIdQuery query, CancellationToken cancellationToken)
        {
            var inmueble = await _inmuebleRepository.GetByIdAsync(query.InmuebleID);
            if (inmueble == null)
            {
                return null;
            }
            return _mapper.Map<InmuebleDto>(inmueble);
        }
    }
}
