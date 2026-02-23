using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories;

public interface IClienteDireccionRepository
{
    /// <summary>
    /// Obtiene todas las direcciones de un cliente
    /// </summary>
    Task<IEnumerable<ClienteDireccion>> GetByClienteIdAsync(Guid clienteId);

    /// <summary>
    /// Obtiene una dirección específica por su ID
    /// </summary>
    Task<ClienteDireccion?> GetByIdAsync(Guid clienteDireccionId);

    /// <summary>
    /// Obtiene la dirección principal de un cliente
    /// </summary>
    Task<ClienteDireccion?> GetPrincipalByClienteIdAsync(Guid clienteId);

    /// <summary>
    /// Obtiene todas las direcciones activas de un cliente
    /// </summary>
    Task<IEnumerable<ClienteDireccion>> GetActivasByClienteIdAsync(Guid clienteId);

    /// <summary>
    /// Agrega una nueva dirección
    /// </summary>
    Task AddAsync(ClienteDireccion direccion);

    /// <summary>
    /// Actualiza una dirección existente
    /// </summary>
    Task UpdateAsync(ClienteDireccion direccion);

    /// <summary>
    /// Elimina una dirección (eliminación lógica - marca como inactiva)
    /// </summary>
    Task DeleteAsync(Guid clienteDireccionId);

    /// <summary>
    /// Marca una dirección como principal
    /// Desmarca otras direcciones del mismo cliente
    /// </summary>
    Task SetAsPrincipalAsync(Guid clienteDireccionId);

    /// <summary>
    /// Verifica si un cliente tiene direcciones
    /// </summary>
    Task<bool> HasDireccionesAsync(Guid clienteId);
}
