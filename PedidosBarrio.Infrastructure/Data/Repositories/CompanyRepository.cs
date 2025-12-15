using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class CompanyRepository : GenericRepository, ICompanyRepository
    {
        public CompanyRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task AddAsync(Company company)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", company.Id);
                parameters.Add("@Name", company.Name);
                parameters.Add("@Ruc", company.Ruc);
                parameters.Add("@PhoneNumber", company.PhoneNumber);
                parameters.Add("@AddressStreet", company.AddressStreet);
                parameters.Add("@AddressCity", company.AddressCity);
                parameters.Add("@AddressZipCode", company.AddressZipCode);

                await ExecuteAsync(connection, "sp_AddCompany", parameters, CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                await ExecuteAsync(connection, "sp_DeleteCompany", new { Id = id }, CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Company>(connection, "sp_GetAllCompanies", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Company> GetByIdAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                return await QuerySingleOrDefaultAsync<Company>(connection, "sp_GetCompanyById", new { Id = id }, CommandType.StoredProcedure);
            }
        }

        public async Task UpdateAsync(Company company)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", company.Id);
                parameters.Add("@Name", company.Name);
                parameters.Add("@Ruc", company.Ruc);
                parameters.Add("@PhoneNumber", company.PhoneNumber);
                parameters.Add("@AddressStreet", company.AddressStreet);
                parameters.Add("@AddressCity", company.AddressCity);
                parameters.Add("@AddressZipCode", company.AddressZipCode);

                await ExecuteAsync(connection, "sp_UpdateCompany", parameters, CommandType.StoredProcedure);
            }
        }
    }
}