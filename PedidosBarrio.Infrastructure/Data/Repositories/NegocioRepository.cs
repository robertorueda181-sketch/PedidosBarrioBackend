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

        public async Task<Negocio> GetByIdAsync(string id)
        {
            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync(
                    "SELECT * FROM sp_GetNegocioById(@negocioid)",
                    new { negocioid = id },
                    commandType: CommandType.Text);

                var row = result.FirstOrDefault();
                if (row == null)
                    return null;

                return new Negocio(
                    empresaID: (Guid)row.EmpresaID,
                    tiposID: (int)row.TiposID,
                    urlNegocio: (string)row.URLCalculada,
                    descripcion: (string)row.Descripcion
                )
                {
                    NegocioID = (int)row.NegocioID
                };
            }
        }

        public async Task<EmpresaNegocio> GetByCodigoEmpresaAsync(string id)
        {
            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync<EmpresaNegocio>(
                    "SELECT * FROM sp_getnegociobyCodigoEmpresa(@negocioid)",
                    new { negocioid = id },
                    commandType: CommandType.Text);

                var row = result.FirstOrDefault();
                if (row == null)
                    return null;

                return row;
            }
        }

        public async Task<IEnumerable<Negocio>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync(
                    "SELECT * FROM sp_getallnegocios()",
                    commandType: CommandType.Text);

                return result.Select(row => new Negocio(
                    empresaID: (Guid)row.EmpresaID,
                    tiposID: (int)row.TiposID,
                    urlNegocio: (string)row.URLCalculada,
                    descripcion: (string)row.Descripcion
                )
                {
                    NegocioID = (int)row.NegocioID,
                    Imagenes = new Imagen() {URLImagen = (string)row.URLImagen }
                }).ToList();
            }
        }

        public async Task<IEnumerable<Negocio>> GetByEmpresaIdAsync(Guid empresaId)
        {
            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync(
                    "SELECT * FROM sp_GetNegociosByEmpresa(@empresaid)",
                    new { empresaid = empresaId },
                    commandType: CommandType.Text);

                return result.Select(row => new Negocio(
                    empresaID: (Guid)row.EmpresaID,
                    tiposID: (int)row.TiposID,
                    urlNegocio: (string)row.URLCalculada,
                    descripcion: (string)row.Descripcion
                )
                {
                    NegocioID = (int)row.NegocioID
                }).ToList();
            }
        }

        public async Task<int> AddAsync(Negocio negocio)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empresaid", negocio.EmpresaID);
                parameters.Add("@tiposid", negocio.TiposID);
                parameters.Add("@urlnegocio", negocio.URLNegocio);
                parameters.Add("@urlopcional", negocio.URLOpcional);
                parameters.Add("@descripcion", negocio.Descripcion);

                return await QuerySingleOrDefaultAsync<int>(
                    connection,
                    "SELECT sp_CreateNegocio(@empresaid, @tiposid, @urlnegocio, @urlopcional, @descripcion)",
                    parameters,
                    CommandType.Text);
            }
        }

        public async Task UpdateAsync(Negocio negocio)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@negocioid", negocio.NegocioID);
                parameters.Add("@empresaid", negocio.EmpresaID);
                parameters.Add("@tiposid", negocio.TiposID);
                parameters.Add("@urlnegocio", negocio.URLNegocio);
                parameters.Add("@urlopcional", negocio.URLOpcional);
                parameters.Add("@descripcion", negocio.Descripcion);

                await ExecuteAsync(
                    connection,
                    "SELECT sp_UpdateNegocio(@negocioid, @empresaid, @tiposid, @urlnegocio, @urlopcional, @descripcion)",
                    parameters,
                    CommandType.Text);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                await ExecuteAsync(
                    connection,
                    "SELECT sp_DeleteNegocio(@negocioid)",
                    new { negocioid = id },
                    CommandType.Text);
            }
        }
    }
}


