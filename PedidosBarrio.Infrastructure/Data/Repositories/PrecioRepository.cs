using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class PrecioRepository : EfCoreRepository<Precio>, IPrecioRepository
    {
        public PrecioRepository(PedidosBarrioDbContext context) : base(context)
        {
        }

        public async Task<Precio?> GetByIdAsync(int id)
        {
            return await GetByIdAsync<int>(id);
        }

        public async Task<IEnumerable<Precio>> GetByProductoIdAsync(int productoId)
        {
            return await _context.Precios
                .AsNoTracking()
                .Where(p => p.Presentacion.ProductoID == productoId)
                .ToListAsync();
        }

        public async Task<Precio?> GetPrecioActualByProductoIdAsync(int productoId)
        {
            return await _context.Precios
                .AsNoTracking()
                .Where(p => p.Presentacion.ProductoID == productoId)
                // Logic: Principal first
                .OrderByDescending(p => p.Principal) 
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Precio>> GetByEmpresaIdAsync(Guid empresaId)
        {
            return await _context.Precios
                .AsNoTracking()
                .Include(p => p.Presentacion)
                .Where(p => p.EmpresaID == empresaId)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Precio precio)
        {
            // La fecha y el estado activo se manejan con valores por defecto si es necesario

            await base.AddAsync(precio);
            return precio.IdPrecio;
        }

        public async Task UpdateAsync(Precio precio)
        {
            var existing = await GetByIdAsync(precio.IdPrecio);
            if (existing != null)
            {
                existing.PrecioValor = precio.PrecioValor;
                existing.Descripcion = precio.Descripcion;
                existing.Principal = precio.Principal;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var precio = await GetByIdAsync(id);
            if (precio != null)
            {
                _context.Precios.Remove(precio);
                await _context.SaveChangesAsync();
            }
        }
    }
}