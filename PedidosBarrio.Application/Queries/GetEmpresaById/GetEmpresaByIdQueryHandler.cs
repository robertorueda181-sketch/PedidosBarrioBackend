using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetEmpresaById
{
    public class GetEmpresaByIdQueryHandler : IRequestHandler<GetEmpresaByIdQuery, EmpresaDto>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IMapper _mapper;

        public GetEmpresaByIdQueryHandler(IEmpresaRepository empresaRepository, IMapper mapper)
        {
            _empresaRepository = empresaRepository;
            _mapper = mapper;
        }

        public async Task<EmpresaDto> Handle(GetEmpresaByIdQuery query, CancellationToken cancellationToken)
        {
            var empresa = await _empresaRepository.GetByIdAsync(query.EmpresaID);
            if (empresa == null)
            {
                return null;
            }
            return _mapper.Map<EmpresaDto>(empresa);
        }
    }
}
