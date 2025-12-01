using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllNegocios
{
    public class GetAllNegociosQueryHandler : IRequestHandler<GetAllNegociosQuery, IEnumerable<NegocioDto>>
    {
        private readonly INegocioRepository _negocioRepository;
        private readonly IMapper _mapper;

        public GetAllNegociosQueryHandler(INegocioRepository negocioRepository, IMapper mapper)
        {
            _negocioRepository = negocioRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NegocioDto>> Handle(GetAllNegociosQuery query, CancellationToken cancellationToken)
        {
            var negocios = await _negocioRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<NegocioDto>>(negocios);
        }
    }
}
