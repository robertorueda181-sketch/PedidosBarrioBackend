using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories;

/// <summary>
/// Repositorio para gestionar tokens de dispositivos
/// </summary>
public interface IDeviceTokenRepository
{
    /// <summary>
    /// Agrega un nuevo token de dispositivo
    /// </summary>
    Task<int> AddAsync(DeviceToken deviceToken);

    /// <summary>
    /// Obtiene un token por su valor
    /// </summary>
    Task<DeviceToken?> GetByTokenAsync(string token);

    /// <summary>
    /// Obtiene todos los tokens activos
    /// </summary>
    Task<List<DeviceToken>> GetAllActiveAsync();

    /// <summary>
    /// Obtiene todos los tokens activos de una empresa
    /// </summary>
    Task<List<DeviceToken>> GetActiveByEmpresaAsync(Guid empresaId);

    /// <summary>
    /// Obtiene todos los tokens activos de un cliente
    /// </summary>
    Task<List<DeviceToken>> GetActiveByClienteAsync(int clienteId);

    /// <summary>
    /// Actualiza un token de dispositivo
    /// </summary>
    Task<bool> UpdateAsync(DeviceToken deviceToken);

    /// <summary>
    /// Desactiva un token
    /// </summary>
    Task<bool> DeactivateAsync(int tokenId);

    /// <summary>
    /// Desactiva un token por su valor
    /// </summary>
    Task<bool> DeactivateByTokenAsync(string token);

    /// <summary>
    /// Elimina un token
    /// </summary>
    Task<bool> DeleteAsync(int tokenId);

    /// <summary>
    /// Verifica si un token ya existe
    /// </summary>
    Task<bool> ExistsAsync(string token);

    /// <summary>
    /// Obtiene tokens de múltiples dispositivos
    /// </summary>
    Task<List<string>> GetActiveTokensAsync(int skip = 0, int take = 100);
}
