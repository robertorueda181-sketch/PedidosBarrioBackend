using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class ImagenRepository : EfCoreRepository<Imagen>, IImagenRepository
    {
        public ImagenRepository(PedidosBarrioDbContext context) : base(context)
        {
        }

        public async Task<Imagen> GetByIdAsync(int id)
        {
            return await GetByIdAsync<int>(id) ?? throw new KeyNotFoundException($"Imagen with ID {id} not found");
        }

        public async Task<IEnumerable<Imagen>> GetByEmpresaIdAsync(Guid empresaId)
        {
            return await _context.Imagenes
                .Where(i => i.EmpresaID == empresaId && (i.Activa ?? true))
                .OrderBy(i => i.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Imagen>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task<IEnumerable<Imagen>> GetByProductoIdAsync(int productoId, string tipo = "PRODUCT")
        {
            return await _context.Imagenes
                .Where(i => i.ExternalId == productoId && i.Type == tipo && (i.Activa ?? true))
                .OrderBy(i => i.Order)
                .ToListAsync();
        }

        public async Task<Imagen> GetPrincipalByProductoIdAsync(int productoId)
        {
            return await _context.Imagenes
                .Where(i => i.ExternalId == productoId && i.Type == "PRODUCT" && (i.Activa ?? true))
                .OrderBy(i => i.Order)
                .FirstOrDefaultAsync();
        }

        public async Task SetPrincipalAsync(int imagenId, int productoId, Guid empresaId)
        {
            // Set order 0 or 1 for this image, shift others
            var images = await GetByProductoIdAsync(productoId);
            foreach (var img in images)
            {
                if (img.ImagenID == imagenId)
                {
                    img.Order = 0; // Principal
                }
                else if (img.Order <= 0)
                {
                    img.Order = 1;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> AddAsync(Imagen imagen)
        {
            if (!imagen.FechaRegistro.HasValue || imagen.FechaRegistro.Value.Kind != DateTimeKind.Utc) 
                imagen.FechaRegistro = DateTime.UtcNow;

            if (!imagen.Activa.HasValue) imagen.Activa = true;
            if (string.IsNullOrEmpty(imagen.Type)) imagen.Type = "PRODUCT"; // Default if needed

            await base.AddAsync(imagen);
            return imagen.ImagenID;
        }

        public async Task UpdateAsync(Imagen imagen)
        {
            var existing = await GetByIdAsync(imagen.ImagenID);
            if (existing != null)
            {
                existing.Descripcion = imagen.Descripcion;
                existing.Order = imagen.Order;
                existing.Urlimagen = imagen.Urlimagen;
                existing.Activa = imagen.Activa;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var imagen = await _context.Imagenes.FirstOrDefaultAsync(i => i.ImagenID == id); // Use FirstOrDefault to avoid exception in GetById
            if (imagen != null)
            {
                imagen.Activa = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}