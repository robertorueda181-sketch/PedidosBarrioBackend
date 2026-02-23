using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories;

public class PedidoDetalleRepository : IPedidoDetalleRepository
{
    private readonly PedidosBarrioDbContext _context;

    public PedidoDetalleRepository(PedidosBarrioDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PedidoDetalle>> GetByPedidoIdAsync(long pedidoId)
    {
        return await _context.PedidoDetalles
            .AsNoTracking()
            .Where(pd => pd.PedidoID == pedidoId)
            .ToListAsync();
    }

    public async Task AddAsync(PedidoDetalle detalles)
    {
        _context.PedidoDetalles.Add(detalles);
        await _context.SaveChangesAsync();
    }

    public async Task AddBulkAsync(IEnumerable<PedidoDetalle> detalles)
    {
        _context.PedidoDetalles.AddRange(detalles);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByPedidoIdAsync(long pedidoId)
    {
        var detalles = await GetByPedidoIdAsync(pedidoId);
        _context.PedidoDetalles.RemoveRange(detalles);
        await _context.SaveChangesAsync();
    }
}
