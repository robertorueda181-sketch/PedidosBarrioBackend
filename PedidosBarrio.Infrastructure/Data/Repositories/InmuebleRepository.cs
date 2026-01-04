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
                var result = await connection.QueryAsync(
                    "SELECT * FROM sp_GetInmuebleById(@p_inmuebleid)",
                    new { p_inmuebleid = id },
                    commandType: CommandType.Text);

                var row = result.FirstOrDefault();
                if (row == null)
                    return null;

                return new Inmueble(
                    empresaID: (Guid)row.EmpresaID,
                    tiposID: (int)row.TiposID,
                    precio: (decimal)row.Precio,
                    medidas: (string)row.Medidas,
                    ubicacion: (string)row.Ubicacion,
                    dormitorios: (int)row.Dormitorios,
                    banos: (int)row.Banos,
                    descripcion: (string)row.Descripcion
                )
                {
                    InmuebleID = (int)row.InmuebleID,
                    Latitud = ((decimal) row.latitud).ToString(),
                    Longitud = ((decimal) row.longitud).ToString(),
                    Tipo = (string)row.Tipo,
                    Activa = true,
                    Operacion = new Tipo { Descripcion = (string)row.Operacion }
                };
            }
        }

        public async Task<IEnumerable<Inmueble>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync(
                    "SELECT * FROM sp_getallinmuebles()",
                    commandType: CommandType.Text);

                return result.Select(row => new Inmueble(
                    empresaID: (Guid)row.EmpresaID,
                    tiposID: (int)row.TiposID,
                    precio: (decimal)row.Precio,
                    medidas: (string)row.Medidas,
                    ubicacion: (string)row.Ubicacion,
                    dormitorios: (int)row.Dormitorios,
                    banos: (int)row.Banos,
                    descripcion: (string)row.Descripcion
                )
                {
                    InmuebleID = (int)row.InmuebleID,
                    Tipo = (string) row.Tipo,
                    Activa = true,
                    Imagen = new Imagen { URLImagen = (string)row.URLImagen },
                    Operacion = new Tipo { Descripcion = (string)row.Operacion }
                }).ToList();
            }
        }

        /// <summary>
        /// Obtiene todos los inmuebles con sus detalles (Tipo, Operacion, URLImagen)
        /// Devuelve datos enriquecidos directamente desde la FUNCTION sp_getallinmuebles
        /// </summary>
        public async Task<IEnumerable<dynamic>> GetAllWithDetailsAsync()
        {
            using (var connection = CreateConnection())
            {
                // Retorna directamente el resultado con campos: InmuebleID, Tipo, Operacion, URLImagen
                return await connection.QueryAsync(
                    "SELECT * FROM sp_getallinmuebles()",
                    commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<Inmueble>> GetByEmpresaIdAsync(int empresaId)
        {
            using (var connection = CreateConnection())
            {
                var result = await connection.QueryAsync(
                    "SELECT * FROM sp_GetInmueblesByEmpresa(@p_empresaid)",
                    new { p_empresaid = empresaId },
                    commandType: CommandType.Text);

                return result.Select(row => new Inmueble(
                    empresaID: (Guid)row.EmpresaID,
                    tiposID: (int)row.TiposID,
                    precio: (decimal)row.Precio,
                    medidas: (string)row.Medidas,
                    ubicacion: (string)row.Ubicacion,
                    dormitorios: (int)row.Dormitorios,
                    banos: (int)row.Banos,
                    descripcion: (string)row.Descripcion
                )
                {
                    InmuebleID = (int)row.InmuebleID,
                    Tipo = (string)row.Tipo,
                    Activa = true,
                    Imagen = new Imagen { URLImagen = (string)row.URLImagen },
                    Operacion = new Tipo { Descripcion = (string)row.Operacion }
                }).ToList();
            }
        }

        public async Task<int> AddAsync(Inmueble inmueble)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_empresaid", inmueble.EmpresaID);
                parameters.Add("@p_tiposid", inmueble.TiposID);
                parameters.Add("@p_precio", inmueble.Precio);
                parameters.Add("@p_medidas", inmueble.Medidas);
                parameters.Add("@p_ubicacion", inmueble.Ubicacion);
                parameters.Add("@p_dormitorios", inmueble.Dormitorios);
                parameters.Add("@p_banos", inmueble.Banos);
                parameters.Add("@p_descripcion", inmueble.Descripcion);

                return await QuerySingleOrDefaultAsync<int>(
                    connection,
                    "SELECT sp_CreateInmueble(@p_empresaid, @p_tiposid, @p_precio, @p_medidas, @p_ubicacion, @p_dormitorios, @p_banos, @p_descripcion)",
                    parameters,
                    CommandType.Text);
            }
        }

        public async Task UpdateAsync(Inmueble inmueble)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@p_inmuebleid", inmueble.InmuebleID);
                parameters.Add("@p_empresaid", inmueble.EmpresaID);
                parameters.Add("@p_tiposid", inmueble.TiposID);
                parameters.Add("@p_precio", inmueble.Precio);
                parameters.Add("@p_medidas", inmueble.Medidas);
                parameters.Add("@p_ubicacion", inmueble.Ubicacion);
                parameters.Add("@p_dormitorios", inmueble.Dormitorios);
                parameters.Add("@p_banos", inmueble.Banos);
                parameters.Add("@p_descripcion", inmueble.Descripcion);

                await ExecuteAsync(
                    connection,
                    "SELECT sp_UpdateInmueble(@p_inmuebleid, @p_empresaid, @p_tiposid, @p_precio, @p_medidas, @p_ubicacion, @p_dormitorios, @p_banos, @p_descripcion)",
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
                    "SELECT sp_DeleteInmueble(@p_inmuebleid)",
                    new { p_inmuebleid = id },
                    CommandType.Text);
            }
        }
    }
}
