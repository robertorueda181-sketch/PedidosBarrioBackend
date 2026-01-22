using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
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
        private readonly IMapper _mapper;

        public CreateImagenCommandHandler(
            IImagenRepository imagenRepository, 
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
            _currentUserService = currentUserService;
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
            return _mapper.Map<ImagenDto>(imagen);
        }
    }
}
