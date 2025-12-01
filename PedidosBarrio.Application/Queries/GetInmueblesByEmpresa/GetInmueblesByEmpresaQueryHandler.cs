using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetInmueblesByEmpresa
{
    public class GetInmueblesByEmpresaQueryHandler : IRequestHandler<GetInmueblesByEmpresaQuery, IEnumerable<InmuebleDto>>
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IMapper _mapper;

        public GetInmueblesByEmpresaQueryHandler(IInmuebleRepository inmuebleRepository, IMapper mapper)
        {
            _inmuebleRepository = inmuebleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InmuebleDto>> Handle(GetInmueblesByEmpresaQuery query, CancellationToken cancellationToken)
        {
            var inmuebles = await _inmuebleRepository.GetByEmpresaIdAsync(query.EmpresaID);
            return _mapper.Map<IEnumerable<InmuebleDto>>(inmuebles);
        }
    }
}
