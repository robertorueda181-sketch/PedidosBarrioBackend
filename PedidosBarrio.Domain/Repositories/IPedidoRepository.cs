using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories;

public interface IPedidoRepository
{
    Task<Pedido> GetByIdAsync(long id);
    Task<Pedido> GetByUidAsync(Guid pedidoUid);
    Task<IEnumerable<Pedido>> GetByEmpresaIdAsync(Guid empresaId);
    Task<IEnumerable<Pedido>> GetByClienteIdAsync(Guid clienteId);
    Task<long> AddAsync(Pedido pedido);
    Task UpdateAsync(Pedido pedido);
    Task DeleteAsync(long id);
}
