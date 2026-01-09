using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class ProductoRepository : GenericRepository, IProductoRepository
    {
        public ProductoRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await QuerySingleOrDefaultAsync<Producto>(
                    connection,
                    "sp_GetProductoById",
                    new { ProductoID = id },
                    CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Producto>(
                    connection,
                    "sp_GetAllProductos",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<Producto>> GetByEmpresaIdAsync(Guid empresaId)
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Producto>(
                    connection,
                    "SELECT * FROM fn_GetProductosByEmpresaId(@p_empresa_id)",
                    new { p_empresa_id = empresaId },
                    commandType: CommandType.Text);
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

                var result = await QuerySingleOrDefaultAsync<int>(
                    connection,
                    "sp_CreateProducto",
                    parameters,
                    CommandType.StoredProcedure);

                return result;
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

                await ExecuteAsync(
                    connection,
                    "sp_UpdateProducto",
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
                    "sp_DeleteProducto",
                    new { ProductoID = id },
                    CommandType.StoredProcedure);
            }
        }
    }
}
