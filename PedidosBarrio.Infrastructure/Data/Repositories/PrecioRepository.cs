using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class PrecioRepository : GenericRepository, IPrecioRepository
    {
        public PrecioRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<Precio?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();
            return await QuerySingleOrDefaultAsync<Precio>(
                connection,
                "SELECT * FROM sp_GetPrecioById(@p_precio_id)",
                new { p_precio_id = id },
                CommandType.Text);
        }

        public async Task<IEnumerable<Precio>> GetByProductoIdAsync(int productoId)
        {
            using var connection = CreateConnection();
            return await QueryAsync<Precio>(
                connection,
                "SELECT * FROM public.sp_GetPreciosByProducto(@p_producto_id)",
                new { p_producto_id = productoId },
                CommandType.Text);
        }

        public async Task<Precio?> GetPrecioActualByProductoIdAsync(int productoId)
        {
            using var connection = CreateConnection();
            return await QuerySingleOrDefaultAsync<Precio>(
                connection,
                "SELECT * FROM sp_GetPrecioPrincipalByProducto(@p_producto_id)",
                new { p_producto_id = productoId },
                CommandType.Text);
        }

        public async Task<IEnumerable<Precio>> GetByEmpresaIdAsync(Guid empresaId)
        {
            using var connection = CreateConnection();
            return await QueryAsync<Precio>(
                connection,
                "SELECT * FROM sp_GetPrecioByEmpresaId(@p_empresa_id)",
                new { p_empresa_id = empresaId },
                CommandType.Text);
        }

        public async Task<int> AddAsync(Precio precio)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@p_precio_valor", precio.PrecioValor);
            parameters.Add("@p_external_id", precio.ExternalId);
            parameters.Add("@p_empresa_id", precio.EmpresaID);
            parameters.Add("@p_es_principal", precio.EsPrincipal);

            return await QuerySingleOrDefaultAsync<int>(
                connection,
                "SELECT public.sp_CreatePrecio(@p_precio_valor, @p_external_id, @p_empresa_id, @p_es_principal)",
                parameters,
                CommandType.Text);
        }

        public async Task UpdateAsync(Precio precio)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@p_precio_id", precio.IdPrecio);
            parameters.Add("@p_precio_valor", precio.PrecioValor);
            parameters.Add("@p_empresa_id", precio.EmpresaID);
            parameters.Add("@p_es_principal", precio.EsPrincipal);

            await ExecuteAsync(
                connection,
                "SELECT sp_UpdatePrecio(@p_precio_id, @p_precio_valor, @p_empresa_id, @p_descripcion, @p_cantidad_minima, @p_modalidad, @p_es_principal)",
                parameters,
                CommandType.Text);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = CreateConnection();
            await ExecuteAsync(
                connection,
                "SELECT sp_DeletePrecio(@p_precio_id)",
                new { p_precio_id = id },
                CommandType.Text);
        }

        public async Task SetPrincipalAsync(int precioId, int productoId, Guid empresaId)
        {
            using var connection = CreateConnection();
            await ExecuteAsync(
                connection,
                "SELECT sp_SetPrecioPrincipal(@p_precio_id, @p_producto_id, @p_empresa_id)",
                new { p_precio_id = precioId, p_producto_id = productoId, p_empresa_id = empresaId },
                CommandType.Text);
        }

    }
}