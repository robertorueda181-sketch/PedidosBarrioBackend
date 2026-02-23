using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories;

public class ClienteDireccionRepository : IClienteDireccionRepository
{
    private readonly PedidosBarrioDbContext _context;

    public ClienteDireccionRepository(PedidosBarrioDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClienteDireccion>> GetByClienteIdAsync(Guid clienteId)
    {
        return await _context.ClienteDirecciones
            .Where(d => d.ClienteID == clienteId && d.Activa)
            .OrderByDescending(d => d.EsPrincipal)
            .ThenByDescending(d => d.FechaCreacion)
            .ToListAsync();
    }

    public async Task<ClienteDireccion?> GetByIdAsync(Guid clienteDireccionId)
    {
        return await _context.ClienteDirecciones
            .FirstOrDefaultAsync(d => d.ClienteDireccionID == clienteDireccionId);
    }

    public async Task<ClienteDireccion?> GetPrincipalByClienteIdAsync(Guid clienteId)
    {
        return await _context.ClienteDirecciones
            .FirstOrDefaultAsync(d => d.ClienteID == clienteId && d.EsPrincipal && d.Activa);
    }

    public async Task<IEnumerable<ClienteDireccion>> GetActivasByClienteIdAsync(Guid clienteId)
    {
        return await _context.ClienteDirecciones
            .Where(d => d.ClienteID == clienteId && d.Activa)
            .OrderByDescending(d => d.EsPrincipal)
            .ThenByDescending(d => d.FechaCreacion)
            .ToListAsync();
    }

    public async Task AddAsync(ClienteDireccion direccion)
    {
        direccion.ClienteDireccionID = Guid.NewGuid();
        direccion.FechaCreacion = DateTime.UtcNow;
        
        await _context.ClienteDirecciones.AddAsync(direccion);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ClienteDireccion direccion)
    {
        direccion.FechaActualizacion = DateTime.UtcNow;
        
        _context.ClienteDirecciones.Update(direccion);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid clienteDireccionId)
    {
        var direccion = await GetByIdAsync(clienteDireccionId);
        
        if (direccion != null)
        {
            direccion.Activa = false;
            direccion.FechaActualizacion = DateTime.UtcNow;
            
            _context.ClienteDirecciones.Update(direccion);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SetAsPrincipalAsync(Guid clienteDireccionId)
    {
        // Obtener la dirección a marcar como principal
        var direccion = await GetByIdAsync(clienteDireccionId);
        
        if (direccion == null)
            throw new InvalidOperationException($"Dirección con ID {clienteDireccionId} no encontrada");

        // Obtener todas las direcciones del cliente
        var otrasDirecciones = await _context.ClienteDirecciones
            .Where(d => d.ClienteID == direccion.ClienteID && d.ClienteDireccionID != clienteDireccionId)
            .ToListAsync();

        // Desmarcar las otras como principal
        foreach (var otra in otrasDirecciones)
        {
            otra.EsPrincipal = false;
            otra.FechaActualizacion = DateTime.UtcNow;
        }

        // Marcar la nueva dirección como principal
        direccion.EsPrincipal = true;
        direccion.FechaActualizacion = DateTime.UtcNow;

        _context.ClienteDirecciones.UpdateRange(otrasDirecciones);
        _context.ClienteDirecciones.Update(direccion);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasDireccionesAsync(Guid clienteId)
    {
        return await _context.ClienteDirecciones
            .AnyAsync(d => d.ClienteID == clienteId && d.Activa);
    }
}
