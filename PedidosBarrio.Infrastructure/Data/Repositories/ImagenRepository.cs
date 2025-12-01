using Dapper;
using Microsoft.Data.SqlClient;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class ImagenRepository : IImagenRepository
    {
        private readonly string _connectionString;

        public ImagenRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<Imagen> GetByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Imagen>(
                    "sp_GetImagenById",
                    new { ImagenID = id },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Imagen>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Imagen>(
                    "sp_GetAllImagenes",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Imagen>> GetByProductoIdAsync(int productoId)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Imagen>(
                    "sp_GetImagenesByProducto",
                    new { ProductoID = productoId },
                    commandType: CommandType.StoredProcedure);
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

                return await connection.QuerySingleAsync<int>(
                    "sp_CreateImagen",
                    parameters,
                    commandType: CommandType.StoredProcedure);
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

                await connection.ExecuteAsync(
                    "sp_UpdateImagen",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(
                    "sp_DeleteImagen",
                    new { ImagenID = id },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
