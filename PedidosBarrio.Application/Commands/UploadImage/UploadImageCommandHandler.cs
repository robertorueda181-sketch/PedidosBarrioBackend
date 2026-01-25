using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UploadImage
{
    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, ImageResponseDto>
    {
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IImagenRepository _imagenRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageModerationService _imageModerationService;
        private readonly IProductoRepository _productoRepository;
        private readonly IIaModeracionLogRepository _iaModeracionLogRepository;
        private readonly IApplicationLogger _logger;

        public UploadImageCommandHandler(
            IImageProcessingService imageProcessingService,
            IImagenRepository imagenRepository,
            ICurrentUserService currentUserService,
            IImageModerationService imageModerationService,
            IProductoRepository productoRepository,
            IIaModeracionLogRepository iaModeracionLogRepository,
            IApplicationLogger logger)
        {
            _imageProcessingService = imageProcessingService;
            _imagenRepository = imagenRepository;
            _currentUserService = currentUserService;
            _imageModerationService = imageModerationService;
            _productoRepository = productoRepository;
            _iaModeracionLogRepository = iaModeracionLogRepository;
            _logger = logger;
        }

        public async Task<ImageResponseDto> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            var empresaId = _currentUserService.GetEmpresaId();

            // 1. Guardar y optimizar imagen
            var relativeUrl = await _imageProcessingService.OptimizeAndSaveImageAsync(
                request.FileStream, 
                request.FileName, 
                request.ProductoId, 
                empresaId);

            var imagen = new Imagen(
                request.ProductoId, 
                relativeUrl, 
                empresaId, 
                request.Descripcion ?? "Imagen de producto")
            {
            };

            var imagenId = await _imagenRepository.AddAsync(imagen);
            
            if (request.SetAsPrincipal)
            {
                await _imagenRepository.SetPrincipalAsync(imagenId, request.ProductoId, empresaId);
            }

            var urlCompleta = await _imageProcessingService.GetImageUrlAsync(relativeUrl);

            // 2. Moderación de IA para la imagen subida
            try
            {
                await _logger.LogInformationAsync($"Iniciando moderación IA para imagen de producto {request.ProductoId}", "UploadImageCommand");
                
                var moderationResult = await _imageModerationService.ValidateImageAsync(urlCompleta);
                
                // Guardar log de moderación
                var iaLog = new IaModeracionLog
                {
                        EmpresaID = empresaId,
                        EsTexto = false,
                        Apropiado = moderationResult.IsAppropriate,
                        Evaluacion = moderationResult.IsAppropriate 
                            ? $"Imagen apropiada (Confianza: {moderationResult.ConfidenceScore:P})" 
                            : $"Imagen inapropiada: {string.Join(", ", moderationResult.ViolationReasons)}",
                        Contexto = relativeUrl,
                        FechaRegistro = DateTime.UtcNow
                    };
                await _iaModeracionLogRepository.AddAsync(iaLog);

                if (!moderationResult.IsAppropriate)
                {
                    await _logger.LogWarningAsync($"Imagen inapropiada detectada para producto {request.ProductoId}. Desaprobando producto.", "UploadImageCommand");
                    
                    var producto = await _productoRepository.GetByIdAsync(request.ProductoId, empresaId);
                    if (producto != null)
                    {
                        producto.Aprobado = false;
                        await _productoRepository.UpdateAsync(producto);
                    }
                }
                else
                {
                    await _logger.LogWarningAsync($"Imagen apropiada detectada para producto {request.ProductoId}. Apropiado producto.", "UploadImageCommand");

                    var producto = await _productoRepository.GetByIdAsync(request.ProductoId, empresaId);
                    if (producto != null)
                    {
                        producto.Aprobado = true;
                        await _productoRepository.UpdateAsync(producto);
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error en moderación IA post-upload: {ex.Message}", ex, "UploadImageCommand");
            }

            return new ImageResponseDto
            {
                ImagenID = imagenId,
                URLImagen = relativeUrl,
                URLCompleta = urlCompleta,
                Descripcion = imagen.Descripcion,
                FechaRegistro = DateTime.UtcNow,
                IsPrincipal = request.SetAsPrincipal
            };
        }
    }
}
