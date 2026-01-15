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
                    "SELECT * FROM fn_GetCategoriaById(@p_categoriaId)",
                    new { p_categoriaId = categoriaId },
                    CommandType.Text);
            }
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync(Guid empresaId)
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

        public async Task<IEnumerable<Categoria>> GetActiveAsync()
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Categoria>(
                    connection,
                    "SELECT * FROM fn_GetActiveCategories()",
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

        public async Task<short> AddAsync(Categoria categoria)
        {
            using (var connection = CreateConnection())
            {
                var categoriaId = await QuerySingleOrDefaultAsync<short>(
                    connection,
                    "SELECT fn_CreateCategoria(@p_empresaID, @p_descripcion, @p_color)",
                    new 
                    { 
                        p_empresaID = categoria.EmpresaID,
                        p_descripcion = categoria.Descripcion,
                        p_color = categoria.Color
                    },
                    CommandType.Text);

                categoria.CategoriaID = categoriaId;
                return categoriaId;
            }
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            using (var connection = CreateConnection())
            {
                await ExecuteAsync(
                    connection,
                    "SELECT sp_updatecategoria(@p_categoriaId, @p_descripcion, @p_color)",
                    new 
                    { 
                        p_categoriaId = categoria.CategoriaID,
                        p_descripcion = categoria.Descripcion,
                        p_color = categoria.Color
                    },
                    CommandType.Text);
            }
        }

        public async Task SoftDeleteAsync(short categoriaId)
        {
            using (var connection = CreateConnection())
            {
                await ExecuteAsync(
                    connection,
                    "SELECT sp_softdeletecategoria(@p_categoriaId)",
                    new { p_categoriaId = categoriaId },
                    CommandType.Text);
            }
        }
    }
}

