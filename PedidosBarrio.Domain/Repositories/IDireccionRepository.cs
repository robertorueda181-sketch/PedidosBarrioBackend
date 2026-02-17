using System.Collections.Generic;
using System.Threading.Tasks;
using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories;

public interface IDireccionRepository
{
    Task<Direccion?> GetByIdAsync(int id);
    Task<IEnumerable<Direccion>> GetByEmpresaIdAsync(Guid empresaId);
    Task<int> AddAsync(Direccion direccion);
    Task UpdateAsync(Direccion direccion);
    Task DeleteAsync(int id);
}
