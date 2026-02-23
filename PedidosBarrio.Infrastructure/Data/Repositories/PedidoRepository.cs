using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly PedidosBarrioDbContext _context;

    public PedidoRepository(PedidosBarrioDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido> GetByIdAsync(long id)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.PedidoDetalles)
            .FirstOrDefaultAsync(p => p.PedidoID == id) ?? throw new InvalidOperationException("Pedido no encontrado");
    }

    public async Task<Pedido> GetByUidAsync(Guid pedidoUid)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Include(p => p.PedidoDetalles)
            .FirstOrDefaultAsync(p => p.PedidoUID == pedidoUid) ?? throw new InvalidOperationException("Pedido no encontrado");
    }

    public async Task<IEnumerable<Pedido>> GetByEmpresaIdAsync(Guid empresaId)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.EmpresaID == empresaId)
            .Include(p => p.PedidoDetalles)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pedido>> GetByClienteIdAsync(Guid clienteId)
    {
        return await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.ClienteID == clienteId)
            .Include(p => p.PedidoDetalles)
            .ToListAsync();
    }

    public async Task<long> AddAsync(Pedido pedido)
    {
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        return pedido.PedidoID;
    }

    public async Task UpdateAsync(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var pedido = await GetByIdAsync(id);
        _context.Pedidos.Remove(pedido);
        await _context.SaveChangesAsync();
    }
}
