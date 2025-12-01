using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateNegocio
{
    public class CreateNegocioCommandHandler : IRequestHandler<CreateNegocioCommand, NegocioDto>
    {
        private readonly INegocioRepository _negocioRepository;
        private readonly IMapper _mapper;

        public CreateNegocioCommandHandler(INegocioRepository negocioRepository, IMapper mapper)
        {
            _negocioRepository = negocioRepository;
            _mapper = mapper;
        }

        public async Task<NegocioDto> Handle(CreateNegocioCommand command, CancellationToken cancellationToken)
        {
            var negocio = new Negocio(command.EmpresaID, command.TiposID, command.URLNegocio, command.Descripcion, command.URLOpcional);

            await _negocioRepository.AddAsync(negocio);
            return _mapper.Map<NegocioDto>(negocio);
        }
    }
}
