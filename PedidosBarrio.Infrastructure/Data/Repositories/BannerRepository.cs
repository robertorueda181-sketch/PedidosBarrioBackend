using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class BannerRepository : EfCoreRepository<Banner>, IBannerRepository
    {
        public BannerRepository(PedidosBarrioDbContext context) : base(context)
        {
        }

        public async Task<Banner> GetByIdAsync(Guid id)
        {
            return await GetByIdAsync<Guid>(id) ?? throw new KeyNotFoundException($"Banner with ID {id} not found");
        }

        public async Task<IEnumerable<Banner>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<IEnumerable<Banner>> GetByEmpresaIdAsync(Guid empresaId)
        {
            return await _context.Banners
                .Where(b => b.EmpresaID == empresaId && (b.Visible ?? true))
                .OrderByDescending(b => b.Prioridad)
                .ToListAsync();
        }

        public async Task<IEnumerable<Banner>> GetActiveByEmpresaIdAsync(Guid empresaId)
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Banners
                .Where(b => b.EmpresaID == empresaId 
                    && (b.Visible ?? true) // Debe ser visible
                    && b.FechaInicio <= today // Debe haber comenzado
                    && b.FechaExpiracion >= today) // No debe haber expirado
                .OrderByDescending(b => b.Prioridad) // Mayor prioridad primero
                .ThenByDescending(b => (b.Aprobado ?? false) ? 1 : 0) // Aprobados primero
                .ThenByDescending(b => (b.Visible ?? false) ? 1 : 0) // Visibles primero
                .ToListAsync();
        }

        public async Task<IEnumerable<Banner>> GetAllActiveAsync()
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Banners
                .Where(b => (b.Visible ?? true) // Debe ser visible
                    && b.FechaInicio <= today // Debe haber comenzado
                    && b.FechaExpiracion >= today) // No debe haber expirado
                .OrderByDescending(b => b.Prioridad) // Mayor prioridad primero
                .ThenByDescending(b => (b.Aprobado ?? false) ? 1 : 0) // Aprobados primero
                .ThenBy(b => b.EmpresaID) // Agrupado por empresa
                .ToListAsync();
        }

        public async Task<Guid> AddAsync(Banner banner)
        {
            if (banner.FechaCreacion == default || banner.FechaCreacion.Kind != DateTimeKind.Utc)
                banner.FechaCreacion = DateTime.UtcNow;

            await base.AddAsync(banner);
            return banner.BannerID;
        }

        public async Task UpdateAsync(Banner banner)
        {
            var existing = await GetByIdAsync(banner.BannerID);
            if (existing != null)
            {
                existing.Titulo = banner.Titulo;
                existing.Descripcion = banner.Descripcion;
                existing.TextoBoton = banner.TextoBoton;
                existing.Link = banner.Link;
                existing.UrlImagen = banner.UrlImagen;
                existing.FechaExpiracion = banner.FechaExpiracion;
                existing.Visible = banner.Visible;
                existing.Aprobado = banner.Aprobado;
                existing.Prioridad = banner.Prioridad;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var banner = await _context.Banners.FirstOrDefaultAsync(b => b.BannerID == id);
            if (banner != null)
            {
                banner.Visible = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
