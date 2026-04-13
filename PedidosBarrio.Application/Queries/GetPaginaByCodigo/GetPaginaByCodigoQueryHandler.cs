using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetPaginaByCodigo
{
    public class GetPaginaByCodigoQueryHandler : IRequestHandler<GetPaginaByCodigoQuery, PaginaDto>
    {
        private readonly INegocioRepository _negocioRepository;
        private readonly IPaginaRepository _paginaRepository;
        private readonly IMapper _mapper;

        public GetPaginaByCodigoQueryHandler(
            INegocioRepository negocioRepository,
            IPaginaRepository paginaRepository,
            IMapper mapper)
        {
            _negocioRepository = negocioRepository;
            _paginaRepository = paginaRepository;
            _mapper = mapper;
        }

        public async Task<PaginaDto> Handle(GetPaginaByCodigoQuery query, CancellationToken cancellationToken)
        {
            // Lookup Negocio by codigo to get the EmpresaID
            var negocio = await _negocioRepository.GetByIdAsync(query.Codigo);
            if (negocio == null || negocio.EmpresaID == null)
            {
                return null;
            }

            // Query Pagina by CodigoEmpresa (EmpresaID)
            var pagina = await _paginaRepository.GetByCodigoEmpresaAsync(negocio.EmpresaID.Value);
            if (pagina == null)
            {
                return null;
            }

            return _mapper.Map<PaginaDto>(pagina);
        }
    }
}
