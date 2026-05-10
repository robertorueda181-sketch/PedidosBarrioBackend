using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories;

public interface IFormularioContactoRepository
{
    Task<FormularioContacto> GetByIdAsync(Guid id);
    Task<IEnumerable<FormularioContacto>> GetAllAsync();
    Task<IEnumerable<FormularioContacto>> GetByEmpresaIdAsync(Guid empresaId);
    Task<IEnumerable<FormularioContacto>> GetByFechaRangeAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<Guid> AddAsync(FormularioContacto formulario);
    Task UpdateAsync(FormularioContacto formulario);
    Task DeleteAsync(Guid id);
}
