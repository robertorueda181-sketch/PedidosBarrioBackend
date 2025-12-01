using Dapper;
using Microsoft.Data.SqlClient;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<Producto> GetByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Producto>(
                    "sp_GetProductoById",
                    new { ProductoID = id },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Producto>(
                    "sp_GetAllProductos",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Producto>> GetByEmpresaIdAsync(int empresaId)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Producto>(
                    "sp_GetProductosByEmpresa",
                    new { EmpresaID = empresaId },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> AddAsync(Producto producto)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@EmpresaID", producto.EmpresaID);
                parameters.Add("@Nombre", producto.Nombre);
                parameters.Add("@Descripcion", producto.Descripcion);

                return await connection.QuerySingleAsync<int>(
                    "sp_CreateProducto",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateAsync(Producto producto)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ProductoID", producto.ProductoID);
                parameters.Add("@EmpresaID", producto.EmpresaID);
                parameters.Add("@Nombre", producto.Nombre);
                parameters.Add("@Descripcion", producto.Descripcion);

                await connection.ExecuteAsync(
                    "sp_UpdateProducto",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(
                    "sp_DeleteProducto",
                    new { ProductoID = id },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
