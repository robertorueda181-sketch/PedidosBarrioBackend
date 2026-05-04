using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class CategoriaRepository : EfCoreRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(PedidosBarrioDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.Descripcion)
                .ToListAsync();
        }

    }
}


