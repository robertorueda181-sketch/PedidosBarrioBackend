using MediatR;
using PedidosBarrio.Application.Commands.ModerateText;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateBanner
{
    public class CreateBannerWithValidationCommandHandler : IRequestHandler<CreateBannerWithValidationCommand, BannerResponseDto>
    {
        private readonly IMediator _mediator;
        private readonly ISuscripcionRepository _suscripcionRepository;
        private readonly IBannerRepository _bannerRepository;
        private readonly IApplicationLogger _logger;

        public CreateBannerWithValidationCommandHandler(
            IMediator mediator,
            ISuscripcionRepository suscripcionRepository,
            IBannerRepository bannerRepository,
            IApplicationLogger logger)
        {
            _mediator = mediator;
            _suscripcionRepository = suscripcionRepository;
            _bannerRepository = bannerRepository;
            _logger = logger;
        }

        public async Task<BannerResponseDto> Handle(CreateBannerWithValidationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Validar contenido con IA
                var textToModerate = $"{request.Titulo} {request.Descripcion} {request.TextoBoton}";
                var moderationCommand = new ModerateTextCommand(textToModerate);
                var moderationResult = await _mediator.Send(moderationCommand, cancellationToken);

                if (!moderationResult.IsAppropriate)
                {
                    await _logger.LogWarningAsync(
                        $"Banner rechazado por moderación de IA para empresa {request.EmpresaID}: {moderationResult.Message}");
                    return new BannerResponseDto
                    {
                        Success = false,
                        Message = $"El contenido del banner no es apropiado: {moderationResult.Message}"
                    };
                }

                // 2. Verificar si el banner es duplicado
                var existingBanners = await _bannerRepository.GetByEmpresaIdAsync(request.EmpresaID);
                var isDuplicate = existingBanners.Any(b =>
                    b.Titulo == request.Titulo &&
                    b.Descripcion == request.Descripcion &&
                    b.Link == request.Link &&
                    (b.Visible ?? true) == true);

                if (isDuplicate)
                {
                    await _logger.LogWarningAsync(
                        $"Banner duplicado detectado para empresa {request.EmpresaID}: {request.Titulo}");
                    return new BannerResponseDto
                    {
                        Success = false,
                        Message = "Un banner con el mismo contenido y link ya existe para esta empresa"
                    };
                }

                // 3. Obtener nivel de suscripción y determinar prioridad
                var suscripciones = await _suscripcionRepository.GetByEmpresaIdAsync(request.EmpresaID);
                var suscripcionActiva = suscripciones
                    .Where(s => s.Activa == true && s.FechaFin > DateTime.UtcNow)
                    .OrderByDescending(s => s.NivelSuscripcion)
                    .FirstOrDefault();

                short prioridad = 1; // Prioridad por defecto (baja)
                bool aprobadoAutomatico = false;

                if (suscripcionActiva != null)
                {
                    // Prioridad basada en nivel de suscripción (1, 2, 3)
                    prioridad = (short)(suscripcionActiva.NivelSuscripcion ?? 3);

                    // Nivel 1 (Premium) - Aprobado automático
                    // Nivel 2 (Plus) - Requiere moderación
                    // Nivel 3 (Basic) - Requiere aprobación manual
                    aprobadoAutomatico = suscripcionActiva.NivelSuscripcion == 1;

                    await _logger.LogInformationAsync(
                        $"Banner para empresa {request.EmpresaID}: Nivel de Suscripción={suscripcionActiva.NivelSuscripcion}, Prioridad={prioridad}, Aprobado Automático={aprobadoAutomatico}");
                }
                else
                {
                    await _logger.LogWarningAsync(
                        $"No se encontró suscripción activa para empresa {request.EmpresaID}. Usando prioridad por defecto.");
                }

                // 4. Crear el comando de creación de banner con los valores determinados
                var createCommand = new CreateBannerWithImageCommand(
                    request.EmpresaID,
                    request.Titulo,
                    request.Descripcion,
                    request.TextoBoton,
                    request.Link,
                    request.Redireccion,
                    request.FechaInicio,
                    request.FechaFin,
                    request.ImagenStream,
                    request.ImagenFileName,
                    visible: true,
                    aprobado: aprobadoAutomatico,
                    prioridad: prioridad,
                    fechaExpiracion: request.FechaFin);

                var result = await _mediator.Send(createCommand, cancellationToken);

                if (result.Success && !aprobadoAutomatico)
                {
                    result.Message = "Banner creado exitosamente. Requiere aprobación manual para ser visible.";
                }
                else if (result.Success && aprobadoAutomatico)
                {
                    result.Message = "Banner creado y aprobado automáticamente (Premium).";
                }

                return result;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error al crear banner con validación: {ex.Message}", ex);
                return new BannerResponseDto
                {
                    Success = false,
                    Message = $"Error al procesar el banner: {ex.Message}"
                };
            }
        }
    }
}
