using MediatR;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.RegisterPageView;

/// <summary>
/// Handler que encola un evento de PageView sin bloquear la respuesta HTTP
/// 1. Resuelve codigoEmpresa a EmpresaID
/// 2. Encola el evento
/// 3. La cola será procesada de forma asíncrona en segundo plano
/// </summary>
public class RegisterPageViewCommandHandler : IRequestHandler<RegisterPageViewCommand, bool>
{
    private readonly IPageViewQueueService _pageViewQueueService;
    private readonly INegocioRepository _negocioRepository;

    public RegisterPageViewCommandHandler(
        IPageViewQueueService pageViewQueueService,
        INegocioRepository negocioRepository)
    {
        _pageViewQueueService = pageViewQueueService;
        _negocioRepository = negocioRepository;
    }

    public async Task<bool> Handle(RegisterPageViewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Resolver codigoEmpresa a EmpresaID
            var negocio = await _negocioRepository.GetByCodigoEmpresaAsync(request.CodigoEmpresa);
            if (negocio == null)
            {
                // Si no existe el negocio, ignorar la visita silenciosamente
                return false;
            }

            var pageViewEvent = new PageViewEvent(
                negocio.EmpresaID, // Usar el EmpresaID resuelto
                request.Url,
                request.Fecha,
                request.UserAgent,
                request.IpAddress,
                request.Referrer
            );

            await _pageViewQueueService.EnqueuePageViewAsync(pageViewEvent);
            return true;
        }
        catch
        {
            // No lanzar excepción para no afectar la respuesta al usuario
            // La visita se ignora silenciosamente
            return false;
        }
    }
}
