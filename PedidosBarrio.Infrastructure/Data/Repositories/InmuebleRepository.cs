using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class InmuebleRepository : EfCoreRepository<Inmueble>, IInmuebleRepository
    {
        public InmuebleRepository(PedidosBarrioDbContext context) : base(context)
        {
        }

        public new async Task<Inmueble?> GetByIdAsync(int id)
        {
            return await _context.Inmuebles
                .AsNoTracking()
                .Include(i => i.Tipos)
                .Include(i => i.Operacion)
                .FirstOrDefaultAsync(i => i.InmuebleID == id);
        }

        public new async Task<IEnumerable<Inmueble>> GetAllAsync()
        {
            return await _context.Inmuebles
                .AsNoTracking()
                .Include(i => i.Tipos)
                .Include(i => i.Operacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inmueble>> GetByEmpresaIdAsync(Guid empresaId)
        {
            return await _context.Inmuebles
                .AsNoTracking()
                .Where(i => i.EmpresaID == empresaId)
                .Include(i => i.Tipos)
                .Include(i => i.Operacion)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Inmueble inmueble)
        {
            await base.AddAsync(inmueble);
            return inmueble.InmuebleID;
        }

        public new async Task UpdateAsync(Inmueble inmueble)
        {
            var existing = await _context.Inmuebles.FindAsync(inmueble.InmuebleID);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(inmueble);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble != null)
            {
                _context.Inmuebles.Remove(inmueble);
                await _context.SaveChangesAsync();
            }
        }
    }
}
