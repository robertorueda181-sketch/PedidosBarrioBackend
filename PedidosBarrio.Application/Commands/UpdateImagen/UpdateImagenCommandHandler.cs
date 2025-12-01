using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateImagen
{
    public class UpdateImagenCommandHandler : IRequestHandler<UpdateImagenCommand, ImagenDto>
    {
        private readonly IImagenRepository _imagenRepository;
        private readonly IMapper _mapper;

        public UpdateImagenCommandHandler(IImagenRepository imagenRepository, IMapper mapper)
        {
            _imagenRepository = imagenRepository;
            _mapper = mapper;
        }

        public async Task<ImagenDto> Handle(UpdateImagenCommand command, CancellationToken cancellationToken)
        {
            var imagen = await _imagenRepository.GetByIdAsync(command.ImagenID);
            if (imagen == null)
            {
                throw new ApplicationException($"Imagen with ID {command.ImagenID} not found.");
            }

            imagen = new Imagen(command.ProductoID, command.URLImagen, command.Descripcion)
            {
                ImagenID = command.ImagenID
            };

            await _imagenRepository.UpdateAsync(imagen);
            return _mapper.Map<ImagenDto>(imagen);
        }
    }
}
