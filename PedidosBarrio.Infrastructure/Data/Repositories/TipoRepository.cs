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
                .AsNoTracking()
                .Where(t => (param == null || t.Parametro == param) &&
                            t.Activa)
                .OrderBy(t => t.Tipo1)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tipo>> GetTiposPorParametroAsync()
        {
            var searchParams = new List<string> { "Tipo_Ser", "Tipo_Neg" , "Tipo_Inm"};

            return await _context.Tipos
                .AsNoTracking()
                .Where(t => searchParams.Contains(t.Parametro!))
                .Select(t => new { t.Tipo1, t.Icono })
                .Distinct()
                .OrderBy(x => x.Tipo1)
                .Select(x => new Tipo
                {
                    Tipo1 = x.Tipo1,
                    Icono = x.Icono
                })
                .ToListAsync();
        }
    }
}

