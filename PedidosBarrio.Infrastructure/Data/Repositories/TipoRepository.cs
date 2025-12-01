using Dapper;
using Microsoft.Data.SqlClient;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class TipoRepository : ITipoRepository
    {
        private readonly string _connectionString;

        public TipoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Tipo>> GetByCategoriaAsync(string categoria)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Tipo>(
                    "sp_GetTiposByCategoria",
                    new { Categoria = categoria },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Tipo>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Tipo>(
                    "sp_GetAllTipos",
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
