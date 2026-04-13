using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetPaginaMia
{
    public class GetPaginaMiaQueryHandler : IRequestHandler<GetPaginaMiaQuery, PaginaDto>
    {
        private readonly IPaginaRepository _paginaRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetPaginaMiaQueryHandler(
            IPaginaRepository paginaRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _paginaRepository = paginaRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<PaginaDto> Handle(GetPaginaMiaQuery query, CancellationToken cancellationToken)
        {
            // Get EmpresaID from JWT token
            var empresaId = _currentUserService.GetEmpresaId();

            // Query Pagina by CodigoEmpresa
            var pagina = await _paginaRepository.GetByCodigoEmpresaAsync(empresaId);
            if (pagina == null)
            {
                return null;
            }

            return _mapper.Map<PaginaDto>(pagina);
        }
    }
}
