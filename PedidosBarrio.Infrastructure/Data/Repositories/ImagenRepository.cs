using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class ImagenRepository : GenericRepository, IImagenRepository
    {
        public ImagenRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<Imagen> GetByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await QuerySingleOrDefaultAsync<Imagen>(
                    connection,
                    "SELECT * FROM sp_GetImagenById(@imagenid)",
                    new { imagenid = id },
                    CommandType.Text);
            }
        }

        public async Task<IEnumerable<Imagen>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Imagen>(
                    connection,
                    "SELECT * FROM sp_GetAllImagenes()",
                    commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<Imagen>> GetByProductoIdAsync(int productoId, string tipo)
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Imagen>(
                    connection,
                    "SELECT * FROM sp_GetImagenesByExternalId(@externalid, @tipo)",
                    new { externalid = productoId, tipo = tipo },
                    CommandType.Text);
            }
        }

        public async Task<int> AddAsync(Imagen imagen)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@externalid", imagen.ExternalId);
                parameters.Add("@urli", imagen.URLImagen);
                parameters.Add("@descripcion", imagen.Descripcion);
                parameters.Add("@tipo", imagen.Type ?? "prod");

                return await QuerySingleOrDefaultAsync<int>(
                    connection,
                    "SELECT sp_CreateImagen(@externalid, @urli, @descripcion, @tipo)",
                    parameters,
                    CommandType.Text);
            }
        }

        public async Task UpdateAsync(Imagen imagen)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@imagenid", imagen.ImagenID);
                parameters.Add("@externalid", imagen.ExternalId);
                parameters.Add("@urli", imagen.URLImagen);
                parameters.Add("@descripcion", imagen.Descripcion);
                parameters.Add("@tipo", imagen.Type ?? "prod");

                await ExecuteAsync(
                    connection,
                    "SELECT sp_UpdateImagen(@imagenid, @externalid, @urli, @descripcion, @tipo)",
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
                    "SELECT sp_DeleteImagen(@imagenid)",
                    new { imagenid = id },
                    CommandType.Text);
            }
        }
    }
}
