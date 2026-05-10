using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories;

public class FormularioContactoRepository : EfCoreRepository<FormularioContacto>, IFormularioContactoRepository
{
    public FormularioContactoRepository(PedidosBarrioDbContext context) : base(context)
    {
    }

    public async Task<Guid> AddAsync(FormularioContacto formulario)
    {
        await _dbSet.AddAsync(formulario);
        await _context.SaveChangesAsync();
        return formulario.Id;
    }

    public async Task<IEnumerable<FormularioContacto>> GetByEmpresaIdAsync(Guid empresaId)
    {
        return await _dbSet
            .Where(f => f.EmpresaID == empresaId && f.Activa)
            .OrderByDescending(f => f.FechaRegistro)
            .ToListAsync();
    }

    public async Task<IEnumerable<FormularioContacto>> GetByFechaRangeAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _dbSet
            .Where(f => f.FechaRegistro >= fechaInicio && f.FechaRegistro <= fechaFin && f.Activa)
            .OrderByDescending(f => f.FechaRegistro)
            .ToListAsync();
    }

    public async Task<FormularioContacto> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(f => f.Id == id)
            ?? throw new KeyNotFoundException($"Formulario de contacto con ID {id} no encontrado");
    }

    public async Task DeleteAsync(Guid id)
    {
        var formulario = await GetByIdAsync(id);
        formulario.Activa = false;
        await _context.SaveChangesAsync();
    }
}
