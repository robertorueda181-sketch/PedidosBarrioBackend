using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class EmpresaRepository : GenericRepository, IEmpresaRepository
    {
        public EmpresaRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<Empresa> GetByIdAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                var result = await QuerySingleOrDefaultAsync<Empresa>(
                  connection,
                  "SELECT * FROM sp_GetEmpresaById(@p_id)",
                  new
                  {
                      p_id = id
                  },
                  CommandType.Text);
             
                
                return result;
            }
        }

        public async Task<Empresa> GetByEmailAsync(string email)
        {
            using (var connection = CreateConnection())
            {
                return await QuerySingleOrDefaultAsync<Empresa>(
                    connection,
                    "SELECT * FROM public.\"Empresas\" WHERE \"Email\" = @email",
                    new { email = email },
                    CommandType.Text);
            }
        }

        public async Task<IEnumerable<Empresa>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Empresa>(
                    connection,
                    "sp_GetAllEmpresas",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task AddAsync(Empresa empresa)
        {
            using (var connection = CreateConnection())
            {
                var generatedId = await QuerySingleOrDefaultAsync<Guid>(
                    connection,
                    "SELECT sp_CreateEmpresa(@p_usuarioID,@p_tipoEmpresa)",
                    new
                    {
                        p_usuarioID = empresa.UsuarioID,
                        p_tipoEmpresa = empresa.TipoEmpresa
                    },
                    CommandType.Text);

                empresa.ID = generatedId;
            }
        }


        public async Task DeleteAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                await ExecuteAsync(
                    connection,
                    "sp_DeleteEmpresa",
                    new { EmpresaID = id },
                    CommandType.StoredProcedure);
            }
        }
    }
}

