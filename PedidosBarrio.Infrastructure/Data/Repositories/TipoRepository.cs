using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class TipoRepository : ITipoRepository
    {
        private readonly PedidosBarrioDbContext _context;

        public TipoRepository(PedidosBarrioDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Tipo>> GetByCategoriaAsync(string param)
        {
            return await _context.Tipos
                .Where(t => (param == null || t.Parametro == param) &&
                            t.Activa)
                .OrderBy(t => t.Tipo1)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tipo>> GetTiposPorParametroAsync()
        {
            return await _context.Tipos
                .FromSqlRaw("SELECT * from public.fn_get_tipos_por_parametro()")
                .ToListAsync();
        }
    }
}

