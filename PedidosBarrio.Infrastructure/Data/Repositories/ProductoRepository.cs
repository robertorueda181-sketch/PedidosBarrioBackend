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
            using var connection = CreateConnection();
            return await QuerySingleOrDefaultAsync<Producto>(
                connection,
                "SELECT * FROM public.sp_GetProductoById(@p_producto_id)",
                new { p_producto_id = id },
                CommandType.Text);
        }

        public async Task<IEnumerable<Producto>> GetByEmpresaIdAsync(Guid empresaId)
        {
            using var connection = CreateConnection();
            return await QueryAsync<Producto>(
                connection,
                "SELECT * FROM sp_GetProductosByEmpresa(@p_empresa_id)",
                new { p_empresa_id = empresaId },
                CommandType.Text);
        }

        public async Task<int> AddAsync(Producto producto)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@p_empresa_id", producto.EmpresaID);
            parameters.Add("@p_categoria_id", producto.CategoriaID);
            parameters.Add("@p_nombre", producto.Nombre);
            parameters.Add("@p_descripcion", producto.Descripcion);
            parameters.Add("@p_stock", producto.Stock);
            parameters.Add("@p_stock_minimo", producto.StockMinimo);
            parameters.Add("@p_inventario", producto.Inventario);

            var result = await QuerySingleOrDefaultAsync<int>(
                connection,
                "SELECT public.sp_CreateProducto(@p_empresa_id, @p_categoria_id, @p_nombre, @p_descripcion, @p_stock, @p_stock_minimo, @p_inventario)",
                parameters,
                CommandType.Text);

            return result;
        }

        public async Task UpdateAsync(Producto producto)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@p_producto_id", producto.ProductoID);
            parameters.Add("@p_categoria_id", producto.CategoriaID);
            parameters.Add("@p_nombre", producto.Nombre);
            parameters.Add("@p_descripcion", producto.Descripcion);
            parameters.Add("@p_stock_minimo", producto.StockMinimo);
            parameters.Add("@p_inventario", producto.Inventario);
            parameters.Add("@p_activo", producto.Activa);
            parameters.Add("@p_visible", producto.Visible);

            await ExecuteAsync(
                connection,
                "SELECT public.sp_UpdateProducto(@p_producto_id, @p_empresa_id, @p_categoria_id, @p_nombre, @p_descripcion, @p_stock, @p_stock_minimo, @p_inventario,@p_activo,@p_visible)",
                parameters,
                CommandType.Text);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            await ExecuteAsync(
                connection,
                "SELECT sp_DeleteProducto(@p_producto_id)",
                new { p_producto_id = id },
                CommandType.Text);
        }
    }
}

