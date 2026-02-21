using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories;

public interface IPageViewRepository
{
    /// <summary>
    /// Agrega un nuevo PageView a la base de datos
    /// </summary>
    Task<PageView> AddAsync(PageView pageView);

    /// <summary>
    /// Agrega múltiples PageViews de forma eficiente (bulk insert)
    /// </summary>
    Task<int> AddBulkAsync(IEnumerable<PageView> pageViews);

    /// <summary>
    /// Obtiene estadísticas de visitas por empresa en un rango de fechas
    /// </summary>
    Task<IEnumerable<PageView>> GetByEmpresaAndDateRangeAsync(
        Guid empresaId, 
        DateTime startDate, 
        DateTime endDate);

    /// <summary>
    /// Obtiene el conteo de visitas por empresa
    /// </summary>
    Task<int> GetCountByEmpresaAsync(Guid empresaId);

    /// <summary>
    /// Obtiene las URLs más visitadas de una empresa
    /// </summary>
    Task<IEnumerable<(string Url, int Count)>> GetTopUrlsByEmpresaAsync(
        Guid empresaId, 
        int topCount = 10);

    /// <summary>
    /// Marca los PageViews como procesados
    /// </summary>
    Task<int> MarkAsProcessedAsync(IEnumerable<int> pageViewIds);
}
