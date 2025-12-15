using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class InmuebleRepository : GenericRepository, IInmuebleRepository
    {
        public InmuebleRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<Inmueble> GetByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await QuerySingleOrDefaultAsync<Inmueble>(
                    connection,
                    "sp_GetInmuebleById",
                    new { InmuebleID = id },
                    CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Inmueble>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Inmueble>(
                    connection,
                    "sp_GetAllInmuebles",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Inmueble>> GetByEmpresaIdAsync(int empresaId)
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Inmueble>(
                    connection,
                    "sp_GetInmueblesByEmpresa",
                    new { EmpresaID = empresaId },
                    CommandType.StoredProcedure);
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

                return await QuerySingleOrDefaultAsync<int>(
                    connection,
                    "sp_CreateInmueble",
                    parameters,
                    CommandType.StoredProcedure);
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

                await ExecuteAsync(
                    connection,
                    "sp_UpdateInmueble",
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
                    "sp_DeleteInmueble",
                    new { InmuebleID = id },
                    CommandType.StoredProcedure);
            }
        }
    }
}
