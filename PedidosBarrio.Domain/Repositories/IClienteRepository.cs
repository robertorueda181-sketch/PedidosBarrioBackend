using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories;

public interface IClienteRepository
{
    Task<Cliente> GetByIdAsync(Guid id);
    Task<Cliente> GetByDniAndUsuarioAsync(string dni, Guid usuarioId);
    Task<Cliente> GetByDniAsync(string dni);
    Task<Cliente> GetByEmailAsync(string email);
    Task<Cliente> GetByProviderUserIdAsync(string provider, string providerUserId);
    Task<IEnumerable<Cliente>> GetByUsuarioIdAsync(Guid usuarioId);
    Task AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(Guid id);
}
