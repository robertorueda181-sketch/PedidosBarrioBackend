using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetAllEmpresas
{
    public class GetAllEmpresasQueryHandler : IRequestHandler<GetAllEmpresasQuery, IEnumerable<EmpresaDto>>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IMapper _mapper;

        public GetAllEmpresasQueryHandler(IEmpresaRepository empresaRepository, IMapper mapper)
        {
            _empresaRepository = empresaRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmpresaDto>> Handle(GetAllEmpresasQuery query, CancellationToken cancellationToken)
        {
            var empresas = await _empresaRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmpresaDto>>(empresas);
        }
    }
}
