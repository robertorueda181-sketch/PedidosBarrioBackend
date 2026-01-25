using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateImagen
{
    public class CreateImagenCommandHandler : IRequestHandler<CreateImagenCommand, ImagenDto>
    {
        private readonly IImagenRepository _imagenRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageModerationService _imageModerationService;
        private readonly IProductoRepository _productoRepository;
        private readonly IIaModeracionLogRepository _iaModeracionLogRepository;
        private readonly IApplicationLogger _logger;
        private readonly IMapper _mapper;

        public CreateImagenCommandHandler(
            IImagenRepository imagenRepository, 
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUserService,
            IImageModerationService imageModerationService,
            IProductoRepository productoRepository,
            IIaModeracionLogRepository iaModeracionLogRepository,
            IApplicationLogger logger,
            IMapper mapper)
        {
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
            _currentUserService = currentUserService;
            _imageModerationService = imageModerationService;
            _productoRepository = productoRepository;
            _iaModeracionLogRepository = iaModeracionLogRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ImagenDto> Handle(CreateImagenCommand command, CancellationToken cancellationToken)
        {
            var empresaId = _currentUserService.GetEmpresaId();
            var imageUrl = command.URLImagen;

            // Si es una URL externa, la descargamos y optimizamos
            if (!string.IsNullOrEmpty(imageUrl) && (imageUrl.StartsWith("http") || imageUrl.StartsWith("https")))
            {
                try
                {
                    imageUrl = await _imageProcessingService.OptimizeAndSaveImageFromUrlAsync(
                        imageUrl, 
                        command.ProductoID, 
                        empresaId);
                }
                catch
                {
                    // Si falla, mantenemos la URL original o manejamos el error
                }
            }

            var imagen = new Imagen(command.ProductoID, imageUrl, empresaId, command.Descripcion ?? "");

            await _imagenRepository.AddAsync(imagen);

            // Moderación de IA para la imagen
            try
            {
                var fullUrl = await _imageProcessingService.GetImageUrlAsync(imageUrl);
                var moderationResult = await _imageModerationService.ValidateImageAsync(fullUrl);

                // Guardar log de moderación
                var iaLog = new IaModeracionLog
                {
                        EmpresaID = empresaId,
                        EsTexto = false,
                        Apropiado = moderationResult.IsAppropriate,
                        Evaluacion = moderationResult.IsAppropriate 
                            ? $"Imagen apropiada (Confianza: {moderationResult.ConfidenceScore:P})" 
                            : $"Imagen inapropiada: {string.Join(", ", moderationResult.ViolationReasons)}",
                        Contexto = imageUrl,
                        FechaRegistro = DateTime.UtcNow
                    };
                await _iaModeracionLogRepository.AddAsync(iaLog);

                if (!moderationResult.IsAppropriate)
                {
                    await _logger.LogWarningAsync($"Imagen inapropiada en CreateImagen para producto {command.ProductoID}", "CreateImagenCommand");
                    
                    var producto = await _productoRepository.GetByIdAsync(command.ProductoID, empresaId);
                    if (producto != null)
                    {
                        producto.Aprobado = false;
                        await _productoRepository.UpdateAsync(producto);
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error en moderación IA post-create-imagen: {ex.Message}", ex, "CreateImagenCommand");
            }

            return _mapper.Map<ImagenDto>(imagen);
        }
    }
}
