using MediatR;
using PedidosBarrio.Application.Commands.ModerateText;
using PedidosBarrio.Application.Commands.ValidateImage;
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
                // 1. Validar contenido con IA (texto)
                var textToModerate = $"{request.Titulo} {request.Descripcion} {request.TextoBoton}";
                var moderationCommand = new ModerateTextCommand(textToModerate);
                var moderationResult = await _mediator.Send(moderationCommand, cancellationToken);

                // Determinar si está aprobado basado en evaluación de IA del texto
                bool textoAprobadoPorIA = moderationResult.IsAppropriate;

                if (!textoAprobadoPorIA)
                {
                    await _logger.LogWarningAsync(
                        $"Banner marcado como NO APROBADO por evaluación de IA (texto) para empresa {request.EmpresaID}: {moderationResult.Message}");
                }
                else
                {
                    await _logger.LogInformationAsync(
                        $"Banner texto aprobado por evaluación de IA para empresa {request.EmpresaID}");
                }

                // 2. Validar imagen con IA
                bool imagenAprobadaPorIA = true;
                string imageValidationMessage = string.Empty;

                if (request.ImagenStream != null && request.ImagenStream.Length > 0)
                {
                    try
                    {
                        // Convertir stream a base64
                        byte[] imageBytes = new byte[request.ImagenStream.Length];
                        request.ImagenStream.Read(imageBytes, 0, (int)request.ImagenStream.Length);
                        string base64Image = Convert.ToBase64String(imageBytes);

                        // Enviar a validación de imagen
                        var imageValidationCommand = new ValidateImageCommand(
                            imageUrl: null,
                            base64Image: base64Image,
                            toleranceLevel: "MEDIUM");

                        var imageValidationResult = await _mediator.Send(imageValidationCommand, cancellationToken);

                        imagenAprobadaPorIA = imageValidationResult.IsAppropriate;
                        imageValidationMessage = imageValidationResult.Message;

                        if (!imagenAprobadaPorIA)
                        {
                            await _logger.LogWarningAsync(
                                $"Banner marcado como NO APROBADO por evaluación de IA (imagen) para empresa {request.EmpresaID}: {imageValidationMessage}");
                        }
                        else
                        {
                            await _logger.LogInformationAsync(
                                $"Banner imagen aprobada por evaluación de IA para empresa {request.EmpresaID}");
                        }
                    }
                    catch (Exception ex)
                    {
                        await _logger.LogWarningAsync(
                            $"Error al validar imagen del banner para empresa {request.EmpresaID}: {ex.Message}");
                        // No fallar si hay error en validación de imagen, solo loguear
                        imagenAprobadaPorIA = true;
                    }
                }

                // 3. Banner aprobado solo si TEXTO e IMAGEN están ambos aprobados
                bool aprobadoPorIA = textoAprobadoPorIA && imagenAprobadaPorIA;

                // 4. Verificar si el banner es duplicado
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

                // 5. Obtener nivel de suscripción y determinar prioridad
                var suscripciones = await _suscripcionRepository.GetByEmpresaIdAsync(request.EmpresaID);
                var suscripcionActiva = suscripciones
                    .Where(s => s.Activa == true && 
                               (s.FechaFin == null || s.FechaFin > DateTime.Now))
                    .OrderByDescending(s => s.NivelSuscripcion)
                    .FirstOrDefault();

                short prioridad = 1; // Prioridad por defecto (baja)
                bool aprobadoPorSuscripcion = false;

                if (suscripcionActiva != null)
                {
                    // Prioridad basada en nivel de suscripción (1, 2, 3)
                    prioridad = (short)(suscripcionActiva.NivelSuscripcion ?? 3);

                    // Nivel 1 (Premium) - Se respeta la evaluación de IA (texto + imagen)
                    // Nivel 2 (Plus) - Se respeta la evaluación de IA (texto + imagen)
                    // Nivel 3 (Basic) - Se respeta la evaluación de IA (texto + imagen)
                    aprobadoPorSuscripcion = aprobadoPorIA;

                    await _logger.LogInformationAsync(
                        $"Banner para empresa {request.EmpresaID}: Nivel de Suscripción={suscripcionActiva.NivelSuscripcion}, Prioridad={prioridad}, Aprobado (IA - Texto)={textoAprobadoPorIA}, Aprobado (IA - Imagen)={imagenAprobadaPorIA}");
                }
                else
                {
                    await _logger.LogWarningAsync(
                        $"No se encontró suscripción activa para empresa {request.EmpresaID}. Usando prioridad por defecto. Aprobado (IA - Texto)={textoAprobadoPorIA}, Aprobado (IA - Imagen)={imagenAprobadaPorIA}");
                    aprobadoPorSuscripcion = aprobadoPorIA;
                }

                // 6. Crear el comando de creación de banner con los valores determinados
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
                    aprobado: aprobadoPorSuscripcion,  // ✅ Basado en evaluación de IA (texto + imagen)
                    prioridad: prioridad,
                    fechaExpiracion: request.FechaFin);

                var result = await _mediator.Send(createCommand, cancellationToken);

                if (result.Success)
                {
                    if (aprobadoPorSuscripcion)
                    {
                        result.Message = "Banner creado y aprobado automáticamente (evaluación de IA - texto e imagen positiva).";
                    }
                    else
                    {
                        var reasons = new List<string>();
                        if (!textoAprobadoPorIA) reasons.Add("texto");
                        if (!imagenAprobadaPorIA) reasons.Add("imagen");

                        result.Message = $"Banner creado pero rechazado por evaluación de IA ({string.Join(" y ", reasons)}). Requiere revisión manual.";
                    }
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
