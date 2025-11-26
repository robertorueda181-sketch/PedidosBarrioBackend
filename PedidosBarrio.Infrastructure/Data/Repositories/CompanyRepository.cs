using Dapper;
using Microsoft.Data.SqlClient;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly string _connectionString;

        public CompanyRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

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

                await connection.ExecuteAsync("sp_AddCompany", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync("sp_DeleteCompany", new { Id = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Company>("sp_GetAllCompanies", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Company> GetByIdAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Company>("sp_GetCompanyById", new { Id = id }, commandType: CommandType.StoredProcedure);
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

                await connection.ExecuteAsync("sp_UpdateCompany", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}