using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class NegocioRepository : GenericRepository, INegocioRepository
    {
        public NegocioRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<Negocio> GetByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await QuerySingleOrDefaultAsync<Negocio>(
                    connection,
                    "sp_GetNegocioById",
                    new { NegocioID = id },
                    CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Negocio>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Negocio>(
                    connection,
                    "sp_GetAllNegocios",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Negocio>> GetByEmpresaIdAsync(int empresaId)
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Negocio>(
                    connection,
                    "sp_GetNegociosByEmpresa",
                    new { EmpresaID = empresaId },
                    CommandType.StoredProcedure);
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

                return await QuerySingleOrDefaultAsync<int>(
                    connection,
                    "sp_CreateNegocio",
                    parameters,
                    CommandType.StoredProcedure);
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

                await ExecuteAsync(
                    connection,
                    "sp_UpdateNegocio",
                    parameters,
                    CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                await ExecuteAsync(
                    connection,
                    "sp_DeleteNegocio",
                    new { NegocioID = id },
                    CommandType.StoredProcedure);
            }
        }
    }
}
