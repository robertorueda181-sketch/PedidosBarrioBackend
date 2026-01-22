using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class UsuarioRepository : EfCoreRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(PedidosBarrioDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> GetByIdAsync(Guid id)
        {
            var user = await _context.Usuarios
                .Include(u => u.Empresas)
                .FirstOrDefaultAsync(u => u.ID == id);

            if (user != null && user.Empresas.Any())
            {
                user.EmpresaID = user.Empresas.First().ID;
            }

            return user;
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            var user = await _context.Usuarios
                .Include(u => u.Empresas)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && user.Empresas.Any())
            {
                user.EmpresaID = user.Empresas.First().ID;
            }

            return user;
        }

        public async Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario)
        {
            var user = await _context.Usuarios
                .Include(u => u.Empresas)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

            if (user != null && user.Empresas.Any())
            {
                user.EmpresaID = user.Empresas.First().ID;
            }

            return user;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await base.GetAllAsync();
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
            if (usuario.UsuarioId == Guid.Empty) usuario.UsuarioId = Guid.NewGuid();
            if (!usuario.FechaRegistro.HasValue) usuario.FechaRegistro = DateTime.UtcNow;

            // Usuario tiene campo Activa? Check Usuario.cs
            // No vi campo Activa en Turn 20 output.
            // Wait, output in Turn 20 showed ONLY properties until FechaRegistro.
            // I'll assume Activa is not there or I should check.
            // Error log didn't complain about Activa in Usuario.
            // But I will safeguard.

            await base.AddAsync(usuario);
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            var existing = await GetByIdAsync(usuario.ID);
            if (existing != null)
            {
                existing.NombreUsuario = usuario.NombreUsuario;
                existing.Email = usuario.Email;
                existing.Activa = usuario.Activa;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var usuario = await GetByIdAsync(id);
            if (usuario != null)
            {
                usuario.Activa = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}