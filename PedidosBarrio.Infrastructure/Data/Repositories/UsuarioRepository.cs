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
                var generatedId = await QuerySingleOrDefaultAsync<Guid>(
                    connection,
                    "SELECT sp_CreateUsuario(@p_nombre_usuario, @p_email, @p_contrasena_hash, @p_contrasena_salt, @p_provider, @p_social_id)",
                    new
                    {
                        p_nombre_usuario = usuario.NombreUsuario,
                        p_email = usuario.Email,
                        p_contrasena_hash = usuario.ContrasenaHash,
                        p_contrasena_salt = usuario.ContrasenaSalt,
                        p_provider = usuario.Provider,
                        p_social_id = usuario.SocialId
                    },
                    CommandType.Text);

                usuario.ID = generatedId;
            }
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            using (var connection = CreateConnection())
            {
                await ExecuteAsync(
                    connection,
                    "SELECT sp_UpdateUsuario(@p_usuario_id, @p_nombre_usuario, @p_email, @p_activa)",
                    new
                    {
                        p_usuario_id = usuario.ID,
                        p_nombre_usuario = usuario.NombreUsuario,
                        p_email = usuario.Email,
                        p_activa = usuario.Activa
                    },
                    CommandType.Text);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var connection = CreateConnection())
            {
                await ExecuteAsync(
                    connection,
                    "SELECT sp_DeleteUsuario(@p_usuario_id)",
                    new { p_usuario_id = id },
                    CommandType.Text);
            }
        }
    }
}
