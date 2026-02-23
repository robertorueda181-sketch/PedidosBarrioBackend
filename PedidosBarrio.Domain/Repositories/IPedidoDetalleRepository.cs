using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories;

public interface IPedidoDetalleRepository
{
    Task<IEnumerable<PedidoDetalle>> GetByPedidoIdAsync(long pedidoId);
    Task AddAsync(PedidoDetalle detalles);
    Task AddBulkAsync(IEnumerable<PedidoDetalle> detalles);
    Task DeleteByPedidoIdAsync(long pedidoId);
}
