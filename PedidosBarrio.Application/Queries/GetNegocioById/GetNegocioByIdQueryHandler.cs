using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetNegocioById
{
    public class GetNegocioByIdQueryHandler : IRequestHandler<GetNegocioByIdQuery, NegocioDto>
    {
        private readonly INegocioRepository _negocioRepository;
        private readonly IMapper _mapper;

        public GetNegocioByIdQueryHandler(INegocioRepository negocioRepository, IMapper mapper)
        {
            _negocioRepository = negocioRepository;
            _mapper = mapper;
        }

        public async Task<NegocioDto> Handle(GetNegocioByIdQuery query, CancellationToken cancellationToken)
        {
            var negocio = await _negocioRepository.GetByIdAsync(query.NegocioID);
            if (negocio == null)
            {
                return null;
            }
            return _mapper.Map<NegocioDto>(negocio);
        }
    }
}
