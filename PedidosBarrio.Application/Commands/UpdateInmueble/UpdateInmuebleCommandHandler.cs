using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateInmueble
{
    public class UpdateInmuebleCommandHandler : IRequestHandler<UpdateInmuebleCommand, InmuebleDto>
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IMapper _mapper;

        public UpdateInmuebleCommandHandler(IInmuebleRepository inmuebleRepository, IMapper mapper)
        {
            _inmuebleRepository = inmuebleRepository;
            _mapper = mapper;
        }

        public async Task<InmuebleDto> Handle(UpdateInmuebleCommand command, CancellationToken cancellationToken)
        {
            var inmueble = await _inmuebleRepository.GetByIdAsync(command.InmuebleID);
            if (inmueble == null)
            {
                throw new ApplicationException($"Inmueble with ID {command.InmuebleID} not found.");
            }

            inmueble = new Inmueble(command.EmpresaID, command.TiposID, command.Precio, command.Medidas,
                command.Ubicacion, command.Dormitorios, command.Banos, command.Descripcion)
            {
                InmuebleID = command.InmuebleID,
                FechaRegistro = inmueble.FechaRegistro
            };

            await _inmuebleRepository.UpdateAsync(inmueble);
            return _mapper.Map<InmuebleDto>(inmueble);
        }
    }
}
