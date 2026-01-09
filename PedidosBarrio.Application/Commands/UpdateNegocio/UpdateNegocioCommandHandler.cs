using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateNegocio
{
    public class UpdateNegocioCommandHandler : IRequestHandler<UpdateNegocioCommand, NegocioDto>
    {
        private readonly INegocioRepository _negocioRepository;
        private readonly IMapper _mapper;

        public UpdateNegocioCommandHandler(INegocioRepository negocioRepository, IMapper mapper)
        {
            _negocioRepository = negocioRepository;
            _mapper = mapper;
        }

        public async Task<NegocioDto> Handle(UpdateNegocioCommand command, CancellationToken cancellationToken)
        {
            var negocio = await _negocioRepository.GetByIdAsync(command.NegocioID.ToString());
            if (negocio == null)
            {
                throw new ApplicationException($"Negocio with ID {command.NegocioID} not found.");
            }

            negocio = new Negocio(command.EmpresaID, command.TiposID, command.URLNegocio, command.Descripcion, command.URLOpcional)
            {
                NegocioID = command.NegocioID,
                FechaRegistro = negocio.FechaRegistro
            };

            await _negocioRepository.UpdateAsync(negocio);
            return _mapper.Map<NegocioDto>(negocio);
        }
    }
}
