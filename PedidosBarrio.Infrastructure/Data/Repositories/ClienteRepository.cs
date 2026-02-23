using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly PedidosBarrioDbContext _context;

    public ClienteRepository(PedidosBarrioDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente> GetByIdAsync(Guid id)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClienteID == id) ?? throw new InvalidOperationException("Cliente no encontrado");
    }

    public async Task<Cliente> GetByDniAndUsuarioAsync(string dni, Guid usuarioId)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.DNI == dni && c.UsuarioID == usuarioId);
    }

    public async Task<Cliente> GetByDniAsync(string dni)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.DNI == dni);
    }

    public async Task<Cliente> GetByEmailAsync(string email)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Cliente> GetByProviderUserIdAsync(string provider, string providerUserId)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Provider == provider && c.ProviderUserId == providerUserId);
    }

    public async Task<IEnumerable<Cliente>> GetByUsuarioIdAsync(Guid usuarioId)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(c => c.UsuarioID == usuarioId && c.Activo)
            .ToListAsync();
    }

    public async Task AddAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var cliente = await GetByIdAsync(id);
        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
    }
}
