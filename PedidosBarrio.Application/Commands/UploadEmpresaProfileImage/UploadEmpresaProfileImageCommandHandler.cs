using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UploadEmpresaProfileImage
{
    public class UploadEmpresaProfileImageCommandHandler : IRequestHandler<UploadEmpresaProfileImageCommand, UploadEmpresaLogoResponseDto>
    {
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IImagenRepository _imagenRepository;
        private readonly IApplicationLogger _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSizeMB = 5;

        public UploadEmpresaProfileImageCommandHandler(
            IImageProcessingService imageProcessingService,
            IImagenRepository imagenRepository,
            IApplicationLogger logger)
        {
            _imageProcessingService = imageProcessingService;
            _imagenRepository = imagenRepository;
            _logger = logger;
        }

        public async Task<UploadEmpresaLogoResponseDto> Handle(UploadEmpresaProfileImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar extensión
                var fileExtension = Path.GetExtension(request.FileName).ToLower();
                if (!_allowedExtensions.Contains(fileExtension))
                {
                    await _logger.LogWarningAsync($"Extensión no permitida para imagen de perfil de empresa {request.EmpresaId}: {fileExtension}");
                    return new UploadEmpresaLogoResponseDto
                    {
                        Success = false,
                        Message = $"Formato no permitido. Extensiones permitidas: {string.Join(", ", _allowedExtensions)}"
                    };
                }

                // Validar tamaño del archivo
                if (request.FileStream.Length > MaxFileSizeMB * 1024 * 1024)
                {
                    await _logger.LogWarningAsync($"Archivo demasiado grande para imagen de perfil de empresa {request.EmpresaId}: {request.FileStream.Length} bytes");
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
                    0, // ProductoId no se usa para imágenes de perfil
                    request.EmpresaId);

                // Crear y guardar registro en base de datos
                var imagen = new Imagen(
                    productoID: null, // No hay producto para imagen de perfil
                    urlImagen: imagePath,
                    empresaID: request.EmpresaId,
                    descripcion: "Imagen de perfil de empresa")
                {
                    Type = "LOGO"
                };

                await _imagenRepository.AddAsync(imagen);

                await _logger.LogInformationAsync($"Imagen de perfil subida exitosamente para empresa {request.EmpresaId}: {imagePath}");

                var imageUrl = await _imageProcessingService.GetImageUrlAsync(imagePath);

                return new UploadEmpresaLogoResponseDto
                {
                    Success = true,
                    Message = "Imagen de perfil cargada y optimizada correctamente",
                    ImagePath = imagePath,
                    ImageUrl = imageUrl
                };
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error al subir imagen de perfil de empresa {request.EmpresaId}: {ex.Message}", ex);
                return new UploadEmpresaLogoResponseDto
                {
                    Success = false,
                    Message = "Error al procesar la imagen. Intente nuevamente."
                };
            }
        }
    }
}
