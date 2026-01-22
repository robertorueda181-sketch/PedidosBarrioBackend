using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetMenusByEmpresa
{
    public class GetMenusByEmpresaQueryHandler : IRequestHandler<GetMenusByEmpresaQuery, IEnumerable<MenuDto>>
    {
        private readonly IMenuRepository _menuRepository;
        
        public GetMenusByEmpresaQueryHandler(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IEnumerable<MenuDto>> Handle(GetMenusByEmpresaQuery request, CancellationToken cancellationToken)
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