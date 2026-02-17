using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UploadEmpresaLogo
{
    public class UploadEmpresaLogoCommandHandler : IRequestHandler<UploadEmpresaLogoCommand, UploadEmpresaLogoResponseDto>
    {
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IImagenRepository _imagenRepository;
        private readonly IApplicationLogger _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSizeMB = 5;

        public UploadEmpresaLogoCommandHandler(
            IImageProcessingService imageProcessingService,
            IImagenRepository imagenRepository,
            IApplicationLogger logger)
        {
            _imageProcessingService = imageProcessingService;
            _imagenRepository = imagenRepository;
            _logger = logger;
        }

        public async Task<UploadEmpresaLogoResponseDto> Handle(UploadEmpresaLogoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar extensión
                var fileExtension = Path.GetExtension(request.FileName).ToLower();
                if (!_allowedExtensions.Contains(fileExtension))
                {
                    await _logger.LogWarningAsync($"Extensión no permitida para logo de empresa {request.EmpresaId}: {fileExtension}");
                    return new UploadEmpresaLogoResponseDto
                    {
                        Success = false,
                        Message = $"Formato no permitido. Extensiones permitidas: {string.Join(", ", _allowedExtensions)}"
                    };
                }

                // Validar tamaño del archivo
                if (request.FileStream.Length > MaxFileSizeMB * 1024 * 1024)
                {
                    await _logger.LogWarningAsync($"Archivo demasiado grande para logo de empresa {request.EmpresaId}: {request.FileStream.Length} bytes");
                    return new UploadEmpresaLogoResponseDto
                    {
                        Success = false,
                        Message = $"El archivo es demasiado grande. Máximo permitido: {MaxFileSizeMB}MB"
                    };
                }

                // Procesar y optimizar imagen
                var imagePath = await _imageProcessingService.OptimizeAndSaveImageAsync(
                    request.FileStream,
                    request.FileName,
                    0, // ProductoId no se usa para logos
                    request.EmpresaId);

                await _logger.LogInformationAsync($"Logo subido exitosamente para empresa {request.EmpresaId}: {imagePath}");

                return new UploadEmpresaLogoResponseDto
                {
                    Success = true,
                    Message = "Logo cargado y optimizado correctamente",
                    ImagePath = imagePath,
                    ImageUrl = await _imageProcessingService.GetImageUrlAsync(imagePath)
                };
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error al subir logo de empresa {request.EmpresaId}: {ex.Message}", ex);
                return new UploadEmpresaLogoResponseDto
                {
                    Success = false,
                    Message = "Error al procesar la imagen. Intente nuevamente."
                };
            }
        }
    }
}
