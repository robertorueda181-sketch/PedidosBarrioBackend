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

        public async Task<Categoria> GetByIdAsync(short categoriaId)
        {
            return await GetByIdAsync<short>(categoriaId);
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync(Guid empresaId)
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(c => c.EmpresaID == empresaId)
                .OrderBy(c => c.Descripcion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Categoria>> GetActiveAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(c => c.Activa.HasValue && c.Activa.Value)
                .OrderBy(c => c.Descripcion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Categoria>> GetByEmpresaIdAsync(Guid empresaId)
        {
            return await GetAllAsync(empresaId);
        }

        public async Task<IEnumerable<Categoria>> GetByEmpresaIdMostrandoAsync(Guid empresaId)
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(c => c.EmpresaID == empresaId && c.Activa.HasValue && c.Activa.Value)
                .OrderBy(c => c.Descripcion)
                .ToListAsync();
        }

        public async Task<short> AddAsync(Categoria categoria)
        {
            if (string.IsNullOrEmpty(categoria.Color)) categoria.Color = "#007bff";
            if (!categoria.Activa.HasValue) categoria.Activa = true;

            await base.AddAsync(categoria);
            return categoria.CategoriaID;
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            var existing = await _context.Categorias.FindAsync(categoria.CategoriaID);
            if (existing != null)
            {
                existing.Descripcion = categoria.Descripcion;
                existing.Color = categoria.Color;
                await _context.SaveChangesAsync();
            }
            else
            {
                 throw new KeyNotFoundException($"Categoria con ID {categoria.CategoriaID} no encontrada");
            }
        }

        public async Task SoftDeleteAsync(short categoriaId)
        {
            var categoria = await _context.Categorias.FindAsync(categoriaId);
            if (categoria != null)
            {
                categoria.Activa = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}


