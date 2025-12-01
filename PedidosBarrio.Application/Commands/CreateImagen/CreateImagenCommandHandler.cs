using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateImagen
{
    public class CreateImagenCommandHandler : IRequestHandler<CreateImagenCommand, ImagenDto>
    {
        private readonly IImagenRepository _imagenRepository;
        private readonly IMapper _mapper;

        public CreateImagenCommandHandler(IImagenRepository imagenRepository, IMapper mapper)
        {
            _imagenRepository = imagenRepository;
            _mapper = mapper;
        }

        public async Task<ImagenDto> Handle(CreateImagenCommand command, CancellationToken cancellationToken)
        {
            var imagen = new Imagen(command.ProductoID, command.URLImagen, command.Descripcion);

            await _imagenRepository.AddAsync(imagen);
            return _mapper.Map<ImagenDto>(imagen);
        }
    }
}
