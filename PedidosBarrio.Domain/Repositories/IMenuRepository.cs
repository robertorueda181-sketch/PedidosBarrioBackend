using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IMenuRepository
    {
        Task<IEnumerable<MenuItem>> GetMenusByEmpresaAsync(Guid empresaId);
    }
}