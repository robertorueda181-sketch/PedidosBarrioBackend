using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class UsuarioRepository : EfCoreRepository<Usuario>, IUsuarioRepository
    {
        private readonly IPiiEncryptionService _encryptionService;

        public UsuarioRepository(PedidosBarrioDbContext context, IPiiEncryptionService encryptionService) : base(context)
        {
            _encryptionService = encryptionService;
        }

        private void EncryptUser(Usuario user)
        {
            if (user == null) return;
            user.Email = _encryptionService.Encrypt(user.Email);
            user.SocialId = _encryptionService.Encrypt(user.SocialId);
        }

        private void DecryptUser(Usuario user)
        {
            if (user == null) return;

            var entry = _context.Entry(user);
            bool isTracked = entry.State != EntityState.Detached;

            user.Email = _encryptionService.Decrypt(user.Email);
            user.SocialId = _encryptionService.Decrypt(user.SocialId);

            // Si el objeto está siendo rastreado, avisamos a EF que no guarde estos cambios
            if (isTracked)
            {
                entry.Property(u => u.Email).IsModified = false;
                entry.Property(u => u.SocialId).IsModified = false;
            }
        }

        public async Task<Usuario?> GetByIdAsync(Guid id)
        {
            var user = await _context.Usuarios
                .Include(u => u.Empresas)
                .FirstOrDefaultAsync(u => u.ID == id);

            if (user != null)
            {
                DecryptUser(user);
                if (user.Empresas.Any())
                {
                    user.EmpresaID = user.Empresas.First().ID;
                }
            }

            return user;
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            var encryptedEmail = _encryptionService.Encrypt(email);
            var user = await _context.Usuarios
                .Include(u => u.Empresas)
                .FirstOrDefaultAsync(u => u.Email == encryptedEmail);

            if (user != null)
            {
                if (user.Empresas.Any())
                {
                    user.EmpresaID = user.Empresas.First().ID;
                }
            }

            return user;
        }

        public new async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            var users = await _context.Usuarios.ToListAsync();
            foreach (var user in users)
            {
                DecryptUser(user);
            }
            return users;
        }

        public async Task<IEnumerable<Usuario>> GetByEmpresaIdAsync(Guid empresaID)
        {
            var userId = await _context.Empresas
                .Where(e => e.ID == empresaID)
                .Select(e => e.UsuarioID)
                .FirstOrDefaultAsync();

            if (userId.HasValue)
            {
                var user = await GetByIdAsync(userId.Value);
                if (user != null)
                {
                    return new[] { user };
                }
            }
            return Enumerable.Empty<Usuario>();
        }

        public async Task AddAsync(Usuario usuario)
        {
            if (usuario.ID == Guid.Empty) usuario.ID = Guid.NewGuid();
            if (!usuario.FechaRegistro.HasValue) usuario.FechaRegistro = DateTime.UtcNow;

            var emailPlain = usuario.Email;
            var socialIdPlain = usuario.SocialId;

            // Encrypt for saving
            usuario.Email = _encryptionService.Encrypt(emailPlain);
            usuario.SocialId = _encryptionService.Encrypt(socialIdPlain);

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            // Restaurar valores en memoria para uso posterior en la ejecución pero marcarlos como no modificados
            usuario.Email = emailPlain;
            usuario.SocialId = socialIdPlain;

            var entry = _context.Entry(usuario);
            entry.Property(u => u.Email).IsModified = false;
            entry.Property(u => u.SocialId).IsModified = false;
        }

                public override async Task UpdateAsync(Usuario usuario)
                {
                    var emailPlain = usuario.Email;
                    var socialIdPlain = usuario.SocialId;

                    // Encriptar antes de guardar
                    usuario.Email = _encryptionService.Encrypt(emailPlain);
                    usuario.SocialId = _encryptionService.Encrypt(socialIdPlain);

                    await base.UpdateAsync(usuario);

                    // Restaurar para uso en memoria
                    usuario.Email = emailPlain;
                    usuario.SocialId = socialIdPlain;

                    var entry = _context.Entry(usuario);
                    entry.Property(u => u.Email).IsModified = false;
                    entry.Property(u => u.SocialId).IsModified = false;
                }

                public async Task DeleteAsync(Guid id)
                {
                    var user = await _context.Usuarios.FindAsync(id);
                    if (user != null)
                    {
                        await base.DeleteAsync(user);
                    }
                }
            }
        }
