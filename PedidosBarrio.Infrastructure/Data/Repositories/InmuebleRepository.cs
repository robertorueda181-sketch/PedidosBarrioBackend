using Dapper;
using Microsoft.Data.SqlClient;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class InmuebleRepository : IInmuebleRepository
    {
        private readonly string _connectionString;

        public InmuebleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<Inmueble> GetByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Inmueble>(
                    "sp_GetInmuebleById",
                    new { InmuebleID = id },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Inmueble>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Inmueble>(
                    "sp_GetAllInmuebles",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Inmueble>> GetByEmpresaIdAsync(int empresaId)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Inmueble>(
                    "sp_GetInmueblesByEmpresa",
                    new { EmpresaID = empresaId },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> AddAsync(Inmueble inmueble)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@EmpresaID", inmueble.EmpresaID);
                parameters.Add("@TiposID", inmueble.TiposID);
                parameters.Add("@Precio", inmueble.Precio);
                parameters.Add("@Medidas", inmueble.Medidas);
                parameters.Add("@Ubicacion", inmueble.Ubicacion);
                parameters.Add("@Dormitorios", inmueble.Dormitorios);
                parameters.Add("@Banos", inmueble.Banos);
                parameters.Add("@Descripcion", inmueble.Descripcion);

                return await connection.QuerySingleAsync<int>(
                    "sp_CreateInmueble",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateAsync(Inmueble inmueble)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@InmuebleID", inmueble.InmuebleID);
                parameters.Add("@EmpresaID", inmueble.EmpresaID);
                parameters.Add("@TiposID", inmueble.TiposID);
                parameters.Add("@Precio", inmueble.Precio);
                parameters.Add("@Medidas", inmueble.Medidas);
                parameters.Add("@Ubicacion", inmueble.Ubicacion);
                parameters.Add("@Dormitorios", inmueble.Dormitorios);
                parameters.Add("@Banos", inmueble.Banos);
                parameters.Add("@Descripcion", inmueble.Descripcion);

                await connection.ExecuteAsync(
                    "sp_UpdateInmueble",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(
                    "sp_DeleteInmueble",
                    new { InmuebleID = id },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
