using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.Menu
{
    public class GetMenusByEmpresaCommandHandler : IRequestHandler<GetMenusByEmpresaCommand, IEnumerable<MenuDto>>
    {
        private readonly IMenuRepository _menuRepository;
        public GetMenusByEmpresaCommandHandler(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IEnumerable<MenuDto>> Handle(GetMenusByEmpresaCommand request, CancellationToken cancellationToken)
        {
            var items = await _menuRepository.GetMenusByEmpresaAsync(request.EmpresaId);
            return items.Select(x => new MenuDto
            {
                MenuID = x.MenuID,
                Nombre = x.Nombre,
                icon = x.icon,
                codigo = x.codigo,
                padre = x.padre,
                order = x.order
            });
        }
    }
}