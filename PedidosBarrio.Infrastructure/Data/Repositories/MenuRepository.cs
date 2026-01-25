using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly PedidosBarrioDbContext _context;

        public MenuRepository(PedidosBarrioDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<MenuItem>> GetMenusByEmpresaAsync(Guid empresaId)
        {
            // Obtener el tipo de empresa
            var empresa = await _context.Empresas
                .AsNoTracking()
                .Where(e => e.ID == empresaId)
                .Select(e => new { e.TipoEmpresa })
                .FirstOrDefaultAsync();

            if (empresa == null || !empresa.TipoEmpresa.HasValue)
            {
                return Enumerable.Empty<MenuItem>();
            }

            // Obtener los menús asociados al tipo de empresa
            var menus = await (from m in _context.Menus.AsNoTracking()
                               join mte in _context.MenusTipoEmpresas.AsNoTracking() on m.MenuID equals mte.MenuID
                               where mte.TipoEmpresa == empresa.TipoEmpresa.Value
                               orderby m.Order
                               select new MenuItem
                               {
                                   MenuID = m.MenuID,
                                   Nombre = m.Nombre ?? string.Empty,
                                   icon = m.Icon ?? string.Empty,
                                   codigo = m.Codigo ?? string.Empty,
                                   padre = m.Padre ?? string.Empty,
                                   order = m.Order
                               }).ToListAsync();

            return menus;
        }
    }
}
