using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetNegociosByEmpresa
{
    public class GetNegociosByEmpresaQueryHandler : IRequestHandler<GetNegociosByEmpresaQuery, IEnumerable<NegocioDto>>
    {
        private readonly INegocioRepository _negocioRepository;
        private readonly IMapper _mapper;

        public GetNegociosByEmpresaQueryHandler(INegocioRepository negocioRepository, IMapper mapper)
        {
            _negocioRepository = negocioRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NegocioDto>> Handle(GetNegociosByEmpresaQuery query, CancellationToken cancellationToken)
        {
            var negocios = await _negocioRepository.GetByEmpresaIdAsync(query.EmpresaID);
            return _mapper.Map<IEnumerable<NegocioDto>>(negocios);
        }
    }
}
