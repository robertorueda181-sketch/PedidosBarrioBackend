using Dapper;
using Microsoft.Data.SqlClient;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly string _connectionString;

        public EmpresaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<Empresa> GetByIdAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Empresa>(
                    "sp_GetEmpresaById", 
                    new { EmpresaID = id }, 
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Empresa>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Empresa>(
                    "sp_GetAllEmpresas",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task AddAsync(Empresa empresa)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Nombre", empresa.Nombre);
                parameters.Add("@Descripcion", empresa.Descripcion);
                parameters.Add("@Email", empresa.Email);
                parameters.Add("@ContrasenaHash", empresa.ContrasenaHash);
                parameters.Add("@ContrasenaSalt", empresa.ContrasenaSalt);
                parameters.Add("@Telefono", empresa.Telefono);
                parameters.Add("@Direccion", empresa.Direccion);
                parameters.Add("@Referencia", empresa.Referencia);

                await connection.ExecuteAsync(
                    "sp_CreateEmpresa",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateAsync(Empresa empresa)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@EmpresaID", empresa.ID);
                parameters.Add("@Nombre", empresa.Nombre);
                parameters.Add("@Email", empresa.Email);
                parameters.Add("@Telefono", empresa.Telefono);
                parameters.Add("@Activa", empresa.Activa);

                await connection.ExecuteAsync(
                    "sp_UpdateEmpresa",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(
                    "sp_DeleteEmpresa",
                    new { EmpresaID = id },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
