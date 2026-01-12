using System;
using System.Collections.Generic;
using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.Menu
{
    public class GetMenusByEmpresaCommand : IRequest<IEnumerable<MenuDto>>
    {
        public Guid EmpresaId { get; }
        public GetMenusByEmpresaCommand(Guid empresaId)
        {
            EmpresaId = empresaId;
        }
    }
}