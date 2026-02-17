using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories;

public class DireccionRepository : EfCoreRepository<Direccion>, IDireccionRepository
{
    public DireccionRepository(PedidosBarrioDbContext context) : base(context)
    {
    }

    public async Task<Direccion?> GetByIdAsync(int id)
    {
        return await GetByIdAsync<int>(id);
    }

    public async Task<IEnumerable<Direccion>> GetByEmpresaIdAsync(Guid empresaId)
    {
        return await _context.Direcciones
            .Where(d => d.EmpresaID == empresaId)
            .ToListAsync();
    }

    public async Task<int> AddAsync(Direccion direccion)
    {
        await base.AddAsync(direccion);
        return direccion.DireccionID;
    }

    public new async Task UpdateAsync(Direccion direccion)
    {
        var existing = await _context.Direcciones.FindAsync(direccion.DireccionID);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(direccion);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var direccion = await _context.Direcciones.FindAsync(id);
        if (direccion != null)
        {
            _context.Direcciones.Remove(direccion);
            await _context.SaveChangesAsync();
        }
    }
}
