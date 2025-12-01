using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateInmueble
{
    public class CreateInmuebleCommandHandler : IRequestHandler<CreateInmuebleCommand, InmuebleDto>
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IMapper _mapper;

        public CreateInmuebleCommandHandler(IInmuebleRepository inmuebleRepository, IMapper mapper)
        {
            _inmuebleRepository = inmuebleRepository;
            _mapper = mapper;
        }

        public async Task<InmuebleDto> Handle(CreateInmuebleCommand command, CancellationToken cancellationToken)
        {
            var inmueble = new Inmueble(command.EmpresaID, command.TiposID, command.Precio, command.Medidas, 
                command.Ubicacion, command.Dormitorios, command.Banos, command.Descripcion);

            await _inmuebleRepository.AddAsync(inmueble);
            return _mapper.Map<InmuebleDto>(inmueble);
        }
    }
}
