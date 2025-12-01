using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateEmpresa
{
    public class UpdateEmpresaCommandHandler : IRequestHandler<UpdateEmpresaCommand, EmpresaDto>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IMapper _mapper;

        public UpdateEmpresaCommandHandler(IEmpresaRepository empresaRepository, IMapper mapper)
        {
            _empresaRepository = empresaRepository;
            _mapper = mapper;
        }

        public async Task<EmpresaDto> Handle(UpdateEmpresaCommand command, CancellationToken cancellationToken)
        {
            var empresa = await _empresaRepository.GetByIdAsync(command.EmpresaID);
            if (empresa == null)
            {
                throw new ApplicationException($"Empresa with ID {command.EmpresaID} not found.");
            }

            // Actualizar solo los campos permitidos (no la contraseña)
            empresa.Nombre = command.Nombre;
            empresa.Descripcion = command.Descripcion;
            empresa.Direccion = command.Direccion;
            empresa.Referencia = command.Referencia;
            empresa.Email = command.Email;
            empresa.Telefono = command.Telefono;

            await _empresaRepository.UpdateAsync(empresa);
            return _mapper.Map<EmpresaDto>(empresa);
        }
    }
}
