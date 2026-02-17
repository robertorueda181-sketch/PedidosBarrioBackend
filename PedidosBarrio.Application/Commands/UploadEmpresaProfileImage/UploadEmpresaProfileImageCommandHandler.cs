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
        private readonly IEmpresaRepository _empresaRepository;
        private readonly INegocioRepository _negocioRepository;
        private readonly IApplicationLogger _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSizeMB = 5;

        public UploadEmpresaProfileImageCommandHandler(
            IImageProcessingService imageProcessingService,
            IImagenRepository imagenRepository,
            IEmpresaRepository empresaRepository,
            INegocioRepository negocioRepository,
            IApplicationLogger logger)
        {
            _imageProcessingService = imageProcessingService;
            _imagenRepository = imagenRepository;
            _empresaRepository = empresaRepository;
            _negocioRepository = negocioRepository;
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

                // Obtener la empresa para verificar su tipo
                var empresa = await _empresaRepository.GetByIdAsync(request.EmpresaId);
                int? productoId = null;

                // Si TipoEmpresa == 1, es un negocio, traer el código del negocio
                if (empresa?.TipoEmpresa == 1)
                {
                    var negocio = (await _negocioRepository.GetByEmpresaIdAsync(request.EmpresaId)).FirstOrDefault();
                    if (negocio != null)
                    {
                        productoId = negocio.NegocioID;
                    }
                }

                // Desactivar otras imágenes de perfil para esta empresa
                await _imagenRepository.DeactivateOtherProfileImagesAsync(request.EmpresaId);

                // Crear y guardar registro en base de datos
                var imagen = new Imagen(
                    productoID: productoId,
                    urlImagen: imagePath,
                    empresaID: request.EmpresaId,
                    descripcion: "Imagen de perfil de empresa")
                {
                    Type = "PROFILE"
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
