using MediatR;
using PedidosBarrio.Application.DTOs;
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

        public UploadImageCommandHandler(
            IImageProcessingService imageProcessingService,
            IImagenRepository imagenRepository,
            ICurrentUserService currentUserService)
        {
            _imageProcessingService = imageProcessingService;
            _imagenRepository = imagenRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ImageResponseDto> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            var empresaId = _currentUserService.GetEmpresaId();

            // Aquí se usa el método que el usuario mencionó que no se usaba
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
                // Podríamos setear como principal si es necesario
                // La lógica de principal suele requerir desactivar otras imágenes principales
            };

            var imagenId = await _imagenRepository.AddAsync(imagen);
            
            if (request.SetAsPrincipal)
            {
                await _imagenRepository.SetPrincipalAsync(imagenId, request.ProductoId, empresaId);
            }

            var urlCompleta = await _imageProcessingService.GetImageUrlAsync(relativeUrl);

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
