using MediatR;
using PedidosBarrio.Application.Commands.ModerateText;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateBanner
{
    public class UpdateBannerWithValidationCommandHandler : IRequestHandler<UpdateBannerWithValidationCommand, BannerResponseDto>
    {
        private readonly IMediator _mediator;
        private readonly ISuscripcionRepository _suscripcionRepository;
        private readonly IBannerRepository _bannerRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IApplicationLogger _logger;

        public UpdateBannerWithValidationCommandHandler(
            IMediator mediator,
            ISuscripcionRepository suscripcionRepository,
            IBannerRepository bannerRepository,
            IImageProcessingService imageProcessingService,
            IApplicationLogger logger)
        {
            _mediator = mediator;
            _suscripcionRepository = suscripcionRepository;
            _bannerRepository = bannerRepository;
            _imageProcessingService = imageProcessingService;
            _logger = logger;
        }

        public async Task<BannerResponseDto> Handle(UpdateBannerWithValidationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Obtener el banner existente
                var bannerExistente = await _bannerRepository.GetByIdAsync(request.BannerId);
                if (bannerExistente == null)
                {
                    return new BannerResponseDto
                    {
                        Success = false,
                        Message = "Banner no encontrado"
                    };
                }

                // 2. Verificar que el banner pertenezca a la empresa
                if (bannerExistente.EmpresaID != request.EmpresaID)
                {
                    return new BannerResponseDto
                    {
                        Success = false,
                        Message = "No tiene permiso para actualizar este banner"
                    };
                }

                // 3. Validar contenido con IA (solo si hay nuevo contenido)
                if (!string.IsNullOrEmpty(request.Titulo) || 
                    !string.IsNullOrEmpty(request.Descripcion) || 
                    !string.IsNullOrEmpty(request.TextoBoton))
                {
                    var nuevoTitulo = request.Titulo ?? bannerExistente.Titulo;
                    var nuevaDescripcion = request.Descripcion ?? bannerExistente.Descripcion;
                    var nuevoTextoBoton = request.TextoBoton ?? bannerExistente.TextoBoton;

                    var textToModerate = $"{nuevoTitulo} {nuevaDescripcion} {nuevoTextoBoton}";
                    var moderationCommand = new ModerateTextCommand(textToModerate);
                    var moderationResult = await _mediator.Send(moderationCommand, cancellationToken);

                    if (!moderationResult.IsAppropriate)
                    {
                        await _logger.LogWarningAsync(
                            $"Banner {request.BannerId} rechazado por moderación de IA: {moderationResult.Message}");
                        return new BannerResponseDto
                        {
                            Success = false,
                            Message = $"El contenido del banner no es apropiado: {moderationResult.Message}"
                        };
                    }
                }

                // 4. Actualizar propiedades del banner
                bannerExistente.Titulo = request.Titulo ?? bannerExistente.Titulo;
                bannerExistente.Descripcion = request.Descripcion ?? bannerExistente.Descripcion;
                bannerExistente.TextoBoton = request.TextoBoton ?? bannerExistente.TextoBoton;
                bannerExistente.Link = request.Link ?? bannerExistente.Link;

                if (request.FechaInicio != default)
                    bannerExistente.FechaInicio = request.FechaInicio;

                if (request.FechaFin != default)
                    bannerExistente.FechaExpiracion = request.FechaFin;

                // 5. Procesar imagen si se proporciona una nueva
                if (request.ImagenStream != null && request.ImagenStream.Length > 0)
                {
                    var imagenUrl = await _imageProcessingService.OptimizeAndSaveImageAsync(
                        request.ImagenStream,
                        request.ImagenFileName,
                        0, // No usamos productoId para banners
                        request.EmpresaID);

                    bannerExistente.UrlImagen = imagenUrl;
                }

                // 6. Guardar cambios
                await _bannerRepository.UpdateAsync(bannerExistente);

                await _logger.LogInformationAsync(
                    $"Banner {request.BannerId} actualizado exitosamente para empresa {request.EmpresaID}");

                return new BannerResponseDto
                {
                    Success = true,
                    BannerId = bannerExistente.BannerID,
                    Message = "Banner actualizado exitosamente"
                };
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error al actualizar banner: {ex.Message}", ex);
                return new BannerResponseDto
                {
                    Success = false,
                    Message = $"Error al procesar el banner: {ex.Message}"
                };
            }
        }
    }
}

