using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories;

public interface IPaginaRepository
{

    /// <summary>
    /// Obtiene todas las páginas de una empresa (solo las activas)
    /// </summary>
    Task<Pagina> GetByCodigoEmpresaAsync(Guid codigoEmpresa);

    /// <summary>
    /// Obtiene una página por su ID
    /// </summary>
    Task<Pagina?> GetByIdAsync(Guid paginaId);

    /// <summary>
    /// Crea una nueva página
    /// </summary>
    Task<Pagina> AddAsync(Pagina pagina);

    /// <summary>
    /// Actualiza una página existente
    /// </summary>
    Task<Pagina> UpdateAsync(Pagina pagina);

    /// <summary>
    /// Verifica si existe una página para un código de empresa y código específicos
    /// </summary>
    Task<bool> ExistsAsync(Guid codigoEmpresa);
}
