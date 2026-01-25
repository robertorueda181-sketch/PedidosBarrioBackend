using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class PresentacionRepository : EfCoreRepository<Presentacion>, IPresentacionRepository
    {
        public PresentacionRepository(PedidosBarrioDbContext context) : base(context)
        {
        }

        public async Task<Presentacion?> GetByIdAsync(int id)
        {
            return await GetByIdAsync<int>(id);
        }

        public async Task<IEnumerable<Presentacion>> GetByProductoIdAsync(int productoId)
        {
            return await _context.Presentaciones
                .AsNoTracking()
                .Where(p => p.ProductoID == productoId)
                .Include(p => p.Precios)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Presentacion presentacion)
        {
            await base.AddAsync(presentacion);
            return presentacion.PresentacionID;
        }

        public new async Task UpdateAsync(Presentacion presentacion)
        {
            var existing = await _context.Presentaciones.FindAsync(presentacion.PresentacionID);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(presentacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var presentacion = await _context.Presentaciones.FindAsync(id);
            if (presentacion != null)
            {
                _context.Presentaciones.Remove(presentacion);
                await _context.SaveChangesAsync();
            }
        }
    }
}
