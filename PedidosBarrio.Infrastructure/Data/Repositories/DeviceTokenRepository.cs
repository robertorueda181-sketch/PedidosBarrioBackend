using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories;

/// <summary>
/// Implementación del repositorio para gestionar tokens de dispositivos
/// </summary>
public class DeviceTokenRepository : EfCoreRepository<DeviceToken>, IDeviceTokenRepository
{
    private readonly PedidosBarrioDbContext _context;

    public DeviceTokenRepository(PedidosBarrioDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(DeviceToken deviceToken)
    {
        await _context.AddAsync(deviceToken);
        await _context.SaveChangesAsync();
        return deviceToken.Id;
    }

    public async Task<DeviceToken?> GetByTokenAsync(string token)
    {
        return await _context.DeviceTokens.FirstOrDefaultAsync(d => d.Token == token);
    }

    public async Task<List<DeviceToken>> GetAllActiveAsync()
    {
        return await _context.DeviceTokens
            .Where(d => d.IsActive)
            .ToListAsync();
    }

    public async Task<List<DeviceToken>> GetActiveByEmpresaAsync(Guid empresaId)
    {
        return await _context.DeviceTokens
            .Where(d => d.IsActive && d.EmpresaId == empresaId)
            .ToListAsync();
    }

    public async Task<List<DeviceToken>> GetActiveByClienteAsync(int clienteId)
    {
        return await _context.DeviceTokens
            .Where(d => d.IsActive && d.ClienteId == clienteId)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(DeviceToken deviceToken)
    {
        _context.DeviceTokens.Update(deviceToken);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeactivateAsync(int tokenId)
    {
        var token = await _context.DeviceTokens.FindAsync(tokenId);
        if (token == null) return false;

        token.IsActive = false;
        _context.DeviceTokens.Update(token);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeactivateByTokenAsync(string token)
    {
        var deviceToken = await GetByTokenAsync(token);
        if (deviceToken == null) return false;

        deviceToken.IsActive = false;
        _context.DeviceTokens.Update(deviceToken);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(int tokenId)
    {
        var token = await _context.DeviceTokens.FindAsync(tokenId);
        if (token == null) return false;

        _context.DeviceTokens.Remove(token);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> ExistsAsync(string token)
    {
        return await _context.DeviceTokens.AnyAsync(d => d.Token == token);
    }

    public async Task<List<string>> GetActiveTokensAsync(int skip = 0, int take = 100)
    {
        return await _context.DeviceTokens
            .Where(d => d.IsActive)
            .Skip(skip)
            .Take(take)
            .Select(d => d.Token)
            .ToListAsync();
    }
}
