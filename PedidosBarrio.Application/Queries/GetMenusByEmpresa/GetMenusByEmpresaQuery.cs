using System;
using System.Collections.Generic;
using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Queries.GetMenusByEmpresa
{
    public class GetMenusByEmpresaQuery : IRequest<IEnumerable<MenuDto>>
    {
        public Guid EmpresaId { get; }
        
        public GetMenusByEmpresaQuery(Guid empresaId)
        {
            EmpresaId = empresaId;
        }
    }
}