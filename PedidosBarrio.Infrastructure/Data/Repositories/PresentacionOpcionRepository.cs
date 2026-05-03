using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class PresentacionOpcionRepository : EfCoreRepository<PresentacionOpcion>, IPresentacionOpcionRepository
    {
        public PresentacionOpcionRepository(PedidosBarrioDbContext context) : base(context)
        {
        }

        public async Task<List<PresentacionOpcion>> GetByPresentacionIdAsync(int presentacionId)
        {
            return await _context.PresentacionOpciones
                .Where(po => po.PresentacionID == presentacionId)
                .ToListAsync();
        }

        public async Task<PresentacionOpcion?> GetByIdAsync(int presentacionOpcionId)
        {
            return await _context.PresentacionOpciones
                .FirstOrDefaultAsync(po => po.PresentacionOpcionID == presentacionOpcionId);
        }

        public async Task<List<PresentacionOpcion>> GetActivasByPresentacionIdAsync(int presentacionId)
        {
            return await _context.PresentacionOpciones
                .Where(po => po.PresentacionID == presentacionId && po.Activa)
                .ToListAsync();
        }

        public async Task AddAsync(PresentacionOpcion entity)
        {
            await base.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PresentacionOpcion entity)
        {
            _context.PresentacionOpciones.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int presentacionOpcionId)
        {
            var entity = await GetByIdAsync(presentacionOpcionId);
            if (entity != null)
            {
                _context.PresentacionOpciones.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
