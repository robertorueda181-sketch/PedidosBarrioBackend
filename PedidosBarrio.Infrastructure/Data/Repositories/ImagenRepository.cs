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
                    "sp_GetImagenById",
                    new { ImagenID = id },
                    CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Imagen>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Imagen>(
                    connection,
                    "sp_GetAllImagenes",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Imagen>> GetByProductoIdAsync(int productoId)
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Imagen>(
                    connection,
                    "sp_GetImagenesByProducto",
                    new { ProductoID = productoId },
                    CommandType.StoredProcedure);
            }
        }

        public async Task<int> AddAsync(Imagen imagen)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ProductoID", imagen.ProductoID);
                parameters.Add("@URL_Imagen", imagen.URLImagen);
                parameters.Add("@Descripcion", imagen.Descripcion);

                return await QuerySingleOrDefaultAsync<int>(
                    connection,
                    "sp_CreateImagen",
                    parameters,
                    CommandType.StoredProcedure);
            }
        }

        public async Task UpdateAsync(Imagen imagen)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ImagenID", imagen.ImagenID);
                parameters.Add("@ProductoID", imagen.ProductoID);
                parameters.Add("@URL_Imagen", imagen.URLImagen);
                parameters.Add("@Descripcion", imagen.Descripcion);

                await ExecuteAsync(
                    connection,
                    "sp_UpdateImagen",
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
                    "sp_DeleteImagen",
                    new { ImagenID = id },
                    CommandType.StoredProcedure);
            }
        }
    }
}
