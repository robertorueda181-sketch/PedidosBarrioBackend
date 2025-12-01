using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateSuscripcion
{
    public class UpdateSuscripcionCommandHandler : IRequestHandler<UpdateSuscripcionCommand, SuscripcionDto>
    {
        private readonly ISuscripcionRepository _suscripcionRepository;
        private readonly IMapper _mapper;

        public UpdateSuscripcionCommandHandler(ISuscripcionRepository suscripcionRepository, IMapper mapper)
        {
            _suscripcionRepository = suscripcionRepository;
            _mapper = mapper;
        }

        public async Task<SuscripcionDto> Handle(UpdateSuscripcionCommand command, CancellationToken cancellationToken)
        {
            var suscripcion = await _suscripcionRepository.GetByIdAsync(command.SuscripcionID);
            if (suscripcion == null)
            {
                throw new ApplicationException($"Suscripcion with ID {command.SuscripcionID} not found.");
            }

            suscripcion = new Suscripcion(command.EmpresaID, command.Monto, command.FechaFin)
            {
                SuscripcionID = command.SuscripcionID,
                Activa = command.Activa,
                FechaInicio = suscripcion.FechaInicio
            };

            await _suscripcionRepository.UpdateAsync(suscripcion);
            return _mapper.Map<SuscripcionDto>(suscripcion);
        }
    }
}
