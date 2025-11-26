using PedidosBarrio.Domain.Entities;


namespace PedidosBarrio.Domain.Repositories
{
    public interface ICompanyRepository
    {
        Task AddAsync(Company company); // Empresa -> Company>
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Company>> GetAllAsync();
        Task<Company> GetByIdAsync(Guid id);
        Task UpdateAsync(Company company);
    }
}
