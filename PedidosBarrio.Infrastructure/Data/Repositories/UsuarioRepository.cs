using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class UsuarioRepository : GenericRepository, IUsuarioRepository
    {
        public UsuarioRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<Usuario> GetByIdAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                return await QuerySingleOrDefaultAsync<Usuario>(
                    connection,
                    "SELECT * FROM sp_GetUsuarioById(@usuarioID)",
                    new { usuarioID = id },
                    CommandType.Text);
            }
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            using (var connection = CreateConnection())
            {
                return await QuerySingleOrDefaultAsync<Usuario>(
                    connection,
                    "SELECT * FROM sp_GetUsuarioByEmail(@email)",
                    new { email = email },
                    CommandType.Text);
            }
        }

        public async Task<Usuario> GetByNombreUsuarioAsync(string nombreUsuario)
        {
            using (var connection = CreateConnection())
            {
                return await QuerySingleOrDefaultAsync<Usuario>(
                    connection,
                    "SELECT * FROM sp_GetUsuarioByNombreUsuario(@nombreUsuario)",
                    new { nombreUsuario = nombreUsuario },
                    CommandType.Text);
            }
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Usuario>(
                    connection,
                    "SELECT * FROM sp_GetAllUsuarios()",
                    commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<Usuario>> GetByEmpresaIdAsync(Guid empresaID)
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Usuario>(
                    connection,
                    "SELECT * FROM sp_GetUsuariosByEmpresaId(@empresaID)",
                    new { empresaID = empresaID },
                    CommandType.Text);
            }
        }

        public async Task AddAsync(Usuario usuario)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@NombreUsuario", usuario.NombreUsuario);
                parameters.Add("@Email", usuario.Email);
                parameters.Add("@ContrasenaHash", usuario.ContrasenaHash);
                parameters.Add("@ContrasenaSalt", usuario.ContrasenaSalt);
                parameters.Add("@EmpresaID", usuario.EmpresaID);
                parameters.Add("@UsuarioID", usuario.ID, dbType: DbType.Guid, direction: ParameterDirection.Output);

                await ExecuteAsync(
                    connection,
                    "sp_CreateUsuario",
                    parameters,
                    CommandType.StoredProcedure);

                var generatedId = parameters.Get<Guid>("@UsuarioID");
                usuario.ID = generatedId;
            }
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            using (var connection = CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UsuarioID", usuario.ID);
                parameters.Add("@NombreUsuario", usuario.NombreUsuario);
                parameters.Add("@Email", usuario.Email);
                parameters.Add("@Activa", usuario.Activa);

                await ExecuteAsync(
                    connection,
                    "sp_UpdateUsuario",
                    parameters,
                    CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                await ExecuteAsync(
                    connection,
                    "sp_DeleteUsuario",
                    new { UsuarioID = id },
                    CommandType.StoredProcedure);
            }
        }
    }
}
