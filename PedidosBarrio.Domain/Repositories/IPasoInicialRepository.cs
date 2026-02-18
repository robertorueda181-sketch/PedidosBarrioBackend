using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories;

public interface IPasoInicialRepository
{
    Task<IEnumerable<PasoInicial>> GetPasosPorEmpresaAsync(Guid empresaId);
    Task<PasoInicial> GetByIdAsync(int pasoId);
    Task<int> AddAsync(PasoInicial paso);
    Task UpdateAsync(PasoInicial paso);
    Task DeleteAsync(int pasoId);
    Task<bool> CompletarPasoAsync(int pasoId);
    Task CrearPasosInicialesDefaultAsync(Guid empresaId);
    Task<bool> TienePasosPendientesAsync(Guid empresaId);
    Task<PasoInicial> GetPasoPorCodigoAsync(Guid empresaId, string codigo);
}
