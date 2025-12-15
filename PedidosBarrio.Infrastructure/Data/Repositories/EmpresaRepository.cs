using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class EmpresaRepository : GenericRepository, IEmpresaRepository
    {
        public EmpresaRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<Empresa> GetByIdAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                return await QuerySingleOrDefaultAsync<Empresa>(
                    connection,
                    "sp_GetEmpresaById", 
                    new { EmpresaID = id }, 
                    CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Empresa>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Empresa>(
                    connection,
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
                parameters.Add("@Descripcion", empresa.Descripcion ?? (object)DBNull.Value);
                parameters.Add("@Email", empresa.Email);
                parameters.Add("@ContrasenaHash", empresa.ContrasenaHash);
                parameters.Add("@ContrasenaSalt", empresa.ContrasenaSalt);
                parameters.Add("@Telefono", empresa.Telefono);
                parameters.Add("@Direccion", empresa.Direccion ?? (object)DBNull.Value);
                parameters.Add("@Referencia", empresa.Referencia ?? (object)DBNull.Value);
                parameters.Add("@EmpresaID", dbType: DbType.Guid, direction: ParameterDirection.Output);

                await ExecuteAsync(
                    connection,
                    "sp_CreateEmpresa",
                    parameters,
                    CommandType.StoredProcedure);

                var generatedId = parameters.Get<Guid>("@EmpresaID");
                empresa.ID = generatedId;
            }
        }

        public async Task UpdateAsync(Empresa empresa)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@EmpresaID", empresa.ID);
                parameters.Add("@Nombre", empresa.Nombre);
                parameters.Add("@Descripcion", empresa.Descripcion ?? (object)DBNull.Value);
                parameters.Add("@Email", empresa.Email);
                parameters.Add("@ContrasenaHash", empresa.ContrasenaHash);
                parameters.Add("@ContrasenaSalt", empresa.ContrasenaSalt);
                parameters.Add("@Telefono", empresa.Telefono);
                parameters.Add("@Direccion", empresa.Direccion ?? (object)DBNull.Value);
                parameters.Add("@Referencia", empresa.Referencia ?? (object)DBNull.Value);
                parameters.Add("@Activa", empresa.Activa);

                await ExecuteAsync(
                    connection,
                    "sp_UpdateEmpresa",
                    parameters,
                    CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                await ExecuteAsync(
                    connection,
                    "sp_DeleteEmpresa",
                    new { EmpresaID = id },
                    CommandType.StoredProcedure);
            }
        }
    }
}

