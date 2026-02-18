using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreatePasoInicial;

public class CreatePasoInicialCommandHandler : IRequestHandler<CreatePasoInicialCommand, PasoInicialDto>
{
    private readonly IPasoInicialRepository _pasoInicialRepository;
    private readonly IMapper _mapper;

    public CreatePasoInicialCommandHandler(
        IPasoInicialRepository pasoInicialRepository,
        IMapper mapper)
    {
        _pasoInicialRepository = pasoInicialRepository;
        _mapper = mapper;
    }

    public async Task<PasoInicialDto> Handle(CreatePasoInicialCommand request, CancellationToken cancellationToken)
    {
        var pasoInicial = new PasoInicial
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            Icono = request.Icono,
            Ruta = request.Ruta,
            Obligatorio = request.Obligatorio,
            Orden = request.Orden,
            Completado = false,
            Activo = true
        };

        var pasoId = await _pasoInicialRepository.AddAsync(pasoInicial);
        pasoInicial.PasoID = pasoId;

        return _mapper.Map<PasoInicialDto>(pasoInicial);
    }
}
