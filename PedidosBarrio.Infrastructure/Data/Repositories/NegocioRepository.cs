using Dapper;
using Microsoft.Data.SqlClient;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class NegocioRepository : INegocioRepository
    {
        private readonly string _connectionString;

        public NegocioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<Negocio> GetByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Negocio>(
                    "sp_GetNegocioById",
                    new { NegocioID = id },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Negocio>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Negocio>(
                    "sp_GetAllNegocios",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Negocio>> GetByEmpresaIdAsync(int empresaId)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Negocio>(
                    "sp_GetNegociosByEmpresa",
                    new { EmpresaID = empresaId },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> AddAsync(Negocio negocio)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@EmpresaID", negocio.EmpresaID);
                parameters.Add("@TiposID", negocio.TiposID);
                parameters.Add("@URL_NEGOCIO", negocio.URLNegocio);
                parameters.Add("@URL_Opcional", negocio.URLOpcional);
                parameters.Add("@Descripcion", negocio.Descripcion);

                return await connection.QuerySingleAsync<int>(
                    "sp_CreateNegocio",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateAsync(Negocio negocio)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@NegocioID", negocio.NegocioID);
                parameters.Add("@EmpresaID", negocio.EmpresaID);
                parameters.Add("@TiposID", negocio.TiposID);
                parameters.Add("@URL_NEGOCIO", negocio.URLNegocio);
                parameters.Add("@URL_Opcional", negocio.URLOpcional);
                parameters.Add("@Descripcion", negocio.Descripcion);

                await connection.ExecuteAsync(
                    "sp_UpdateNegocio",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(
                    "sp_DeleteNegocio",
                    new { NegocioID = id },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
