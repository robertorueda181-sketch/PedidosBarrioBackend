using MediatR;
using PedidosBarrio.Application.DTOs;
using System;

namespace PedidosBarrio.Application.Commands.CreateEmpresa
{
    public class CreateEmpresaCommand : IRequest<EmpresaDto>
    {
        public Guid UsuarioID { get; set; }
        public short TipoEmpresa { get; set; }

        public CreateEmpresaCommand(Guid usuarioID, short tipoEmpresa)
        {
            usuarioID = UsuarioID;
            TipoEmpresa = tipoEmpresa;

        }
    }
}
