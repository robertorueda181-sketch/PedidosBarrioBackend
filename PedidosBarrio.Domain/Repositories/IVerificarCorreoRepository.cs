using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IVerificarCorreoRepository
    {
        Task AddAsync(VerificarCorreo verif);
        Task<VerificarCorreo> GetValidCodeAsync(string correo, string codigo);
        Task DeleteAsync(VerificarCorreo verif);
    }
}
