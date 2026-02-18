using MediatR;

namespace PedidosBarrio.Application.Commands.CompletarPasoInicial;

public class CompletarPasoInicialCommand : IRequest<bool>
{
    public int PasoId { get; set; }

    public CompletarPasoInicialCommand(int pasoId)
    {
        PasoId = pasoId;
    }
}
