using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class SuscripcionRepository : EfCoreRepository<Suscripcion>, ISuscripcionRepository
    {
        public SuscripcionRepository(PedidosBarrioDbContext context) : base(context)
        {
        }

        public async Task<Suscripcion> GetByIdAsync(int id)
        {
            return await GetByIdAsync<int>(id);
        }

        public async Task<IEnumerable<Suscripcion>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<IEnumerable<Suscripcion>> GetByEmpresaIdAsync(Guid empresaId)
        {
            return await _context.Suscripciones
                .Where(s => s.EmpresaID == empresaId)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Suscripcion suscripcion)
        {
            if (suscripcion.FechaRegistro == null || suscripcion.FechaRegistro.Value.Kind != DateTimeKind.Utc)
                suscripcion.FechaRegistro = DateTime.UtcNow;

            if (suscripcion.FechaInicio == null || suscripcion.FechaInicio.Value.Kind != DateTimeKind.Utc)
                suscripcion.FechaInicio = DateTime.UtcNow;

            if (suscripcion.FechaFin.HasValue && suscripcion.FechaFin.Value.Kind != DateTimeKind.Utc)
            {
                suscripcion.FechaFin = suscripcion.FechaFin.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(suscripcion.FechaFin.Value, DateTimeKind.Utc)
                    : suscripcion.FechaFin.Value.ToUniversalTime();
            }

            await _context.Suscripciones.AddAsync(suscripcion);
            await _context.SaveChangesAsync();
            return suscripcion.SuscripcionID;
        }

        public async Task UpdateAsync(Suscripcion suscripcion)
        {
            _context.Entry(suscripcion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var suscripcion = await GetByIdAsync(id);
            if (suscripcion != null)
            {
                _context.Suscripciones.Remove(suscripcion);
                await _context.SaveChangesAsync();
            }
        }
    }
}
