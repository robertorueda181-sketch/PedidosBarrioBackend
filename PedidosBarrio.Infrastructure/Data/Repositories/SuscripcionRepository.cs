using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class SuscripcionRepository : GenericRepository, ISuscripcionRepository
    {
        public SuscripcionRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<Suscripcion> GetByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Suscripcion>(
                    "sp_GetSuscripcionById",
                    new { SuscripcionID = id },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Suscripcion>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Suscripcion>(
                    "sp_GetAllSuscripciones",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Suscripcion>> GetByEmpresaIdAsync(int empresaId)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Suscripcion>(
                    "sp_GetSuscripcionesByEmpresa",
                    new { EmpresaID = empresaId },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> AddAsync(Suscripcion suscripcion)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@EmpresaID", suscripcion.EmpresaID);
                parameters.Add("@FechaFin", suscripcion.FechaFin);
                parameters.Add("@Activa", suscripcion.Activa);
                parameters.Add("@Monto", suscripcion.Monto);

                return await connection.QuerySingleAsync<int>(
                    "sp_CreateSuscripcion",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateAsync(Suscripcion suscripcion)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@SuscripcionID", suscripcion.SuscripcionID);
                parameters.Add("@EmpresaID", suscripcion.EmpresaID);
                parameters.Add("@FechaFin", suscripcion.FechaFin);
                parameters.Add("@Activa", suscripcion.Activa);
                parameters.Add("@Monto", suscripcion.Monto);

                await connection.ExecuteAsync(
                    "sp_UpdateSuscripcion",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(
                    "sp_DeleteSuscripcion",
                    new { SuscripcionID = id },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
