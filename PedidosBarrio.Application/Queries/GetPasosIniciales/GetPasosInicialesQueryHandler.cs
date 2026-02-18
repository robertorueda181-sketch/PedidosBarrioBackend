using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetPasosIniciales;

public class GetPasosInicialesQueryHandler : IRequestHandler<GetPasosInicialesQuery, IEnumerable<PasoInicialDto>>
{
    private readonly IPasoInicialRepository _pasoInicialRepository;
    private readonly IMapper _mapper;

    public GetPasosInicialesQueryHandler(
        IPasoInicialRepository pasoInicialRepository,
        IMapper mapper)
    {
        _pasoInicialRepository = pasoInicialRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PasoInicialDto>> Handle(GetPasosInicialesQuery request, CancellationToken cancellationToken)
    {
        var pasos = await _pasoInicialRepository.GetPasosPorEmpresaAsync(request.EmpresaId);
        return _mapper.Map<IEnumerable<PasoInicialDto>>(pasos);
    }
}
