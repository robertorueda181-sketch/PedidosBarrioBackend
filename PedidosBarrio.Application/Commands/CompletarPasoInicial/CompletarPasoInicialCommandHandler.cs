using MediatR;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CompletarPasoInicial;

public class CompletarPasoInicialCommandHandler : IRequestHandler<CompletarPasoInicialCommand, bool>
{
    private readonly IPasoInicialRepository _pasoInicialRepository;

    public CompletarPasoInicialCommandHandler(IPasoInicialRepository pasoInicialRepository)
    {
        _pasoInicialRepository = pasoInicialRepository;
    }

    public async Task<bool> Handle(CompletarPasoInicialCommand request, CancellationToken cancellationToken)
    {
        return await _pasoInicialRepository.CompletarPasoAsync(request.PasoId);
    }
}
