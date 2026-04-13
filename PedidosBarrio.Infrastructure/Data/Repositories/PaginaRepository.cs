using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories;

public class PaginaRepository : IPaginaRepository
{
    private readonly PedidosBarrioDbContext _context;

    public PaginaRepository(PedidosBarrioDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todas las páginas de una empresa (solo las activas)
    /// </summary>
    public async Task<Pagina> GetByCodigoEmpresaAsync(Guid codigoEmpresa)
    {
        return await _context.Paginas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.CodigoEmpresa == codigoEmpresa && p.Activa);
    }

    /// <summary>
    /// Obtiene una página por su ID
    /// </summary>
    public async Task<Pagina?> GetByIdAsync(Guid paginaId)
    {
        return await _context.Paginas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PaginaID == paginaId);
    }

    /// <summary>
    /// Crea una nueva página
    /// </summary>
    public async Task<Pagina> AddAsync(Pagina pagina)
    {
        pagina.PaginaID = Guid.NewGuid();
        pagina.FechaCreacion = DateTime.UtcNow;
        pagina.FechaActualizacion = DateTime.UtcNow;

        _context.Paginas.Add(pagina);
        await _context.SaveChangesAsync();

        return pagina;
    }

    /// <summary>
    /// Actualiza una página existente
    /// </summary>
    public async Task<Pagina> UpdateAsync(Pagina pagina)
    {
        pagina.FechaActualizacion = DateTime.UtcNow;

        _context.Paginas.Update(pagina);
        await _context.SaveChangesAsync();

        return pagina;
    }

    /// <summary>
    /// Verifica si existe una página para un código de empresa y código específicos
    /// </summary>
    public async Task<bool> ExistsAsync(Guid codigoEmpresa)
    {
        return await _context.Paginas
            .AsNoTracking()
            .AnyAsync(p => 
                p.CodigoEmpresa == codigoEmpresa && 
                p.Activa);
    }
}
