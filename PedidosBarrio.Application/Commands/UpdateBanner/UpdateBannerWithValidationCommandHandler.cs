using MediatR;
using PedidosBarrio.Application.Commands.ModerateText;
using PedidosBarrio.Application.Commands.ValidateImage;
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
        private readonly IImageSaveStrategyFactory _imageSaveStrategyFactory;
        private readonly IApplicationLogger _logger;

        public UpdateBannerWithValidationCommandHandler(
            IMediator mediator,
            ISuscripcionRepository suscripcionRepository,
            IBannerRepository bannerRepository,
            IImageSaveStrategyFactory imageSaveStrategyFactory,
            IApplicationLogger logger)
        {
            _mediator = mediator;
            _suscripcionRepository = suscripcionRepository;
            _bannerRepository = bannerRepository;
            _imageSaveStrategyFactory = imageSaveStrategyFactory;
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
                bool? textoAprobadoPorIA = null;
                bool? imagenAprobadoPorIA = null;

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

                    // Guardar resultado de evaluación de IA (texto)
                    textoAprobadoPorIA = moderationResult.IsAppropriate;

                    if (!textoAprobadoPorIA.Value)
                    {
                        await _logger.LogWarningAsync(
                            $"Banner {request.BannerId} marcado como NO APROBADO por evaluación de IA (texto): {moderationResult.Message}");
                    }
                    else
                    {
                        await _logger.LogInformationAsync(
                            $"Banner {request.BannerId} texto aprobado por evaluación de IA");
                    }
                }

                // 4. Validar imagen si se proporciona una nueva
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

                        imagenAprobadoPorIA = imageValidationResult.IsAppropriate;

                        if (!imagenAprobadoPorIA.Value)
                        {
                            await _logger.LogWarningAsync(
                                $"Banner {request.BannerId} marcado como NO APROBADO por evaluación de IA (imagen): {imageValidationResult.Message}");
                        }
                        else
                        {
                            await _logger.LogInformationAsync(
                                $"Banner {request.BannerId} imagen aprobada por evaluación de IA");
                        }
                    }
                    catch (Exception ex)
                    {
                        await _logger.LogWarningAsync(
                            $"Error al validar imagen del banner {request.BannerId}: {ex.Message}");
                        // No fallar si hay error en validación de imagen, solo loguear
                        imagenAprobadoPorIA = true;
                    }
                }

                // 5. Determinar estado de aprobación final
                bool? aprobadoPorIA = null;
                if (textoAprobadoPorIA.HasValue || imagenAprobadoPorIA.HasValue)
                {
                    // Si se validó texto e imagen, ambos deben estar aprobados
                    if (textoAprobadoPorIA.HasValue && imagenAprobadoPorIA.HasValue)
                    {
                        aprobadoPorIA = textoAprobadoPorIA.Value && imagenAprobadoPorIA.Value;
                    }
                    // Si solo se validó uno, usar ese resultado
                    else if (textoAprobadoPorIA.HasValue)
                    {
                        aprobadoPorIA = textoAprobadoPorIA;
                    }
                    else if (imagenAprobadoPorIA.HasValue)
                    {
                        aprobadoPorIA = imagenAprobadoPorIA;
                    }
                }

                // 5. Actualizar propiedades del banner
                bannerExistente.Titulo = request.Titulo ?? bannerExistente.Titulo;
                bannerExistente.Descripcion = request.Descripcion ?? bannerExistente.Descripcion;
                bannerExistente.TextoBoton = request.TextoBoton ?? bannerExistente.TextoBoton;
                bannerExistente.Link = request.Link ?? bannerExistente.Link;

                if (request.FechaInicio != default)
                    bannerExistente.FechaInicio = DateOnly.FromDateTime(request.FechaInicio);

                if (request.FechaFin != default)
                    bannerExistente.FechaExpiracion = DateOnly.FromDateTime(request.FechaFin);

                                // 6. Actualizar estado de aprobación basado en evaluación de IA
                                if (aprobadoPorIA.HasValue)
                                {
                                    bannerExistente.Aprobado = aprobadoPorIA.Value;
                                    await _logger.LogInformationAsync(
                                        $"Estado de aprobación del banner {request.BannerId} actualizado a: {aprobadoPorIA.Value} (basado en evaluación de IA - Texto: {textoAprobadoPorIA?.ToString() ?? "no validado"}, Imagen: {imagenAprobadoPorIA?.ToString() ?? "no validado"})");
                                }

                                // 7. Procesar imagen si se proporciona una nueva
                                if (request.ImagenStream != null && request.ImagenStream.Length > 0)
                                {
                                    try
                                    {
                                        var bannerStrategy = _imageSaveStrategyFactory.GetStrategy(ImageType.Banner);
                                        var urlImagen = await bannerStrategy.SaveImageAsync(request.ImagenStream, request.ImagenFileName);
                                        bannerExistente.UrlImagen = urlImagen;
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        await _logger.LogWarningAsync($"Error al procesar imagen: {ex.Message}");
                                        return new BannerResponseDto
                                        {
                                            Success = false,
                                            Message = $"Error al procesar imagen: {ex.Message}"
                                        };
                                    }
                                }
                                // Si no hay archivo pero hay URL, usar la URL directamente
                                else if (!string.IsNullOrEmpty(request.ImagenUrl))
                                {
                                    bannerExistente.UrlImagen = request.ImagenUrl;
                                    await _logger.LogInformationAsync($"URL de imagen de banner guardada directamente: {request.ImagenUrl}");
                                }

                                // 8. Guardar cambios
                                await _bannerRepository.UpdateAsync(bannerExistente);

                                await _logger.LogInformationAsync(
                                    $"Banner {request.BannerId} actualizado exitosamente para empresa {request.EmpresaID}");

                                return new BannerResponseDto
                                {
                                    Success = true,
                                    BannerId = bannerExistente.BannerID,
                                    Message = aprobadoPorIA.HasValue 
                                        ? (aprobadoPorIA.Value 
                                            ? "Banner actualizado y aprobado por evaluación de IA (texto e imagen)." 
                                            : GenerarMensajeRechazo(textoAprobadoPorIA, imagenAprobadoPorIA))
                                        : "Banner actualizado exitosamente"
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

                        private string GenerarMensajeRechazo(bool? textoAprobado, bool? imagenAprobado)
                        {
                            var reasons = new List<string>();
                            if (textoAprobado == false) reasons.Add("texto");
                            if (imagenAprobado == false) reasons.Add("imagen");

                            if (reasons.Count > 0)
                            {
                                return $"Banner actualizado pero rechazado por evaluación de IA ({string.Join(" y ", reasons)}). Requiere revisión manual.";
                            }

                            return "Banner actualizado pero rechazado por evaluación de IA. Requiere revisión manual.";
                        }
                    }
                }

