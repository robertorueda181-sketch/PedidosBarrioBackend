using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateSuscripcion
{
    public class CreateSuscripcionCommandHandler : IRequestHandler<CreateSuscripcionCommand, SuscripcionDto>
    {
        private readonly ISuscripcionRepository _suscripcionRepository;
        private readonly IMapper _mapper;

        public CreateSuscripcionCommandHandler(ISuscripcionRepository suscripcionRepository, IMapper mapper)
        {
            _suscripcionRepository = suscripcionRepository;
            _mapper = mapper;
        }

        public async Task<SuscripcionDto> Handle(CreateSuscripcionCommand command, CancellationToken cancellationToken)
        {
            var suscripcion = new Suscripcion(command.EmpresaID, command.Monto, command.FechaFin);

            await _suscripcionRepository.AddAsync(suscripcion);
            return _mapper.Map<SuscripcionDto>(suscripcion);
        }
    }
}
