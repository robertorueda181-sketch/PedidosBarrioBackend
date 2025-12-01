using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetSuscripcionesByEmpresa
{
    public class GetSuscripcionesByEmpresaQueryHandler : IRequestHandler<GetSuscripcionesByEmpresaQuery, IEnumerable<SuscripcionDto>>
    {
        private readonly ISuscripcionRepository _suscripcionRepository;
        private readonly IMapper _mapper;

        public GetSuscripcionesByEmpresaQueryHandler(ISuscripcionRepository suscripcionRepository, IMapper mapper)
        {
            _suscripcionRepository = suscripcionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SuscripcionDto>> Handle(GetSuscripcionesByEmpresaQuery query, CancellationToken cancellationToken)
        {
            var suscripciones = await _suscripcionRepository.GetByEmpresaIdAsync(query.EmpresaID);
            return _mapper.Map<IEnumerable<SuscripcionDto>>(suscripciones);
        }
    }
}
