using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class CategoriaRepository : GenericRepository, ICategoriaRepository
    {
        public CategoriaRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<Categoria> GetByIdAsync(short categoriaId)
        {
            using (var connection = CreateConnection())
            {
                return await QuerySingleOrDefaultAsync<Categoria>(
                    connection,
                    "SELECT * FROM \"Categorias\" WHERE \"CategoriaID\" = @CategoriaID",
                    new { CategoriaID = categoriaId },
                    CommandType.Text);
            }
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Categoria>(
                    connection,
                    "SELECT * FROM \"Categorias\" ORDER BY \"Descripcion\"",
                    commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<Categoria>> GetByEmpresaIdAsync(Guid empresaId)
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Categoria>(
                    connection,
                    "SELECT * FROM fn_GetCategoriasByEmpresa(@p_empresa_id)",
                    new { p_empresa_id = empresaId },
                    CommandType.Text);
            }
        }

        public async Task<IEnumerable<Categoria>> GetByEmpresaIdMostrandoAsync(Guid empresaId)
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Categoria>(
                    connection,
                    "SELECT * FROM public.fn_getcategoriasbyempresa(@p_empresa_id)",
                    new { p_empresa_id = empresaId },
                    CommandType.Text);
            }
        }

        public async Task<int> AddAsync(Categoria categoria)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CategoriaID", categoria.CategoriaID);
                parameters.Add("@EmpresaID", categoria.EmpresaID);
                parameters.Add("@Descripcion", categoria.Descripcion);
                parameters.Add("@Codigo", categoria.Codigo ?? (object)DBNull.Value);
                parameters.Add("@Activo", categoria.Activo);
                parameters.Add("@Mostrar", categoria.Mostrar);

                return await ExecuteAsync(
                    connection,
                    "INSERT INTO \"Categorias\" (\"CategoriaID\", \"EmpresaID\", \"Descripcion\", \"Codigo\", \"Activo\", \"Mostrar\") VALUES (@CategoriaID, @EmpresaID, @Descripcion, @Codigo, @Activo, @Mostrar)",
                    parameters,
                    CommandType.Text);
            }
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CategoriaID", categoria.CategoriaID);
                parameters.Add("@EmpresaID", categoria.EmpresaID);
                parameters.Add("@Descripcion", categoria.Descripcion);
                parameters.Add("@Codigo", categoria.Codigo ?? (object)DBNull.Value);
                parameters.Add("@Activo", categoria.Activo);
                parameters.Add("@Mostrar", categoria.Mostrar);

                await ExecuteAsync(
                    connection,
                    "UPDATE \"Categorias\" SET \"EmpresaID\" = @EmpresaID, \"Descripcion\" = @Descripcion, \"Codigo\" = @Codigo, \"Activo\" = @Activo, \"Mostrar\" = @Mostrar WHERE \"CategoriaID\" = @CategoriaID",
                    parameters,
                    CommandType.Text);
            }
        }

        public async Task DeleteAsync(short categoriaId)
        {
            using (var connection = CreateConnection())
            {
                await ExecuteAsync(
                    connection,
                    "DELETE FROM \"Categorias\" WHERE \"CategoriaID\" = @CategoriaID",
                    new { CategoriaID = categoriaId },
                    CommandType.Text);
            }
        }
    }
}
