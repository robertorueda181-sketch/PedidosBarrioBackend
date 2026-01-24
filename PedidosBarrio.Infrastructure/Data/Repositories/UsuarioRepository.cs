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
            user.Email = _encryptionService.Decrypt(user.Email);
            user.SocialId = _encryptionService.Decrypt(user.SocialId);
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
                DecryptUser(user);
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

            // Restore in-memory values for continued use in the transaction/app
            usuario.Email = emailPlain;
            usuario.SocialId = socialIdPlain;

            // IMPORTANT: Tell EF the state in memory is what matches the DB (vía snapshot)
            // or just detach/mark unchanged so subsequent SaveChanges don't overwrite DB with plain text.
            _context.Entry(usuario).State = EntityState.Unchanged;
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            var existing = await _context.Usuarios.FirstOrDefaultAsync(u => u.ID == usuario.ID);
            if (existing != null)
            {
                existing.Email = _encryptionService.Encrypt(usuario.Email);
                existing.SocialId = _encryptionService.Encrypt(usuario.SocialId);
                existing.Activa = usuario.Activa;
                await _context.SaveChangesAsync();

                // Decrypt existing so it stays usable if tracked
                DecryptUser(existing);
                _context.Entry(existing).State = EntityState.Unchanged;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ID == id);
            if (usuario != null)
            {
                usuario.Activa = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}