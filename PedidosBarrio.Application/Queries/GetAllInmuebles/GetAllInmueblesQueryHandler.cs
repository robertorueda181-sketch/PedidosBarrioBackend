using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllInmuebles
{
    public class GetAllInmueblesQueryHandler : IRequestHandler<GetAllInmueblesQuery, IEnumerable<InmuebleDto>>
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IMapper _mapper;

        public GetAllInmueblesQueryHandler(IInmuebleRepository inmuebleRepository, IMapper mapper)
        {
            _inmuebleRepository = inmuebleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InmuebleDto>> Handle(GetAllInmueblesQuery query, CancellationToken cancellationToken)
        {
            var inmuebles = await _inmuebleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<InmuebleDto>>(inmuebles);
        }
    }
}
