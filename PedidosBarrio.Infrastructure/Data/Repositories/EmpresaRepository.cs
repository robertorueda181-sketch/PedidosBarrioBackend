using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;
using PedidosBarrio.Application.Services;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class EmpresaRepository : EfCoreRepository<Empresa>, IEmpresaRepository
    {
        private readonly IPiiEncryptionService _encryptionService;

        public EmpresaRepository(PedidosBarrioDbContext context, IPiiEncryptionService encryptionService) : base(context)
        {
            _encryptionService = encryptionService;
        }

        public async Task<Empresa> GetByIdAsync(Guid id)
        {
            return await GetByIdAsync<Guid>(id);
        }

        public async Task<Empresa> GetByEmailAsync(string email)
        {
            var encryptedEmail = _encryptionService.Encrypt(email);

            // Join con Usuarios por el email encriptado
            var query = from e in _context.Empresas
                        join u in _context.Usuarios on e.UsuarioID equals u.ID
                        where u.Email == encryptedEmail
                        select e;

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Empresa>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }

        public async Task AddAsync(Empresa empresa)
        {
            if (empresa.ID == Guid.Empty) 
                empresa.ID = Guid.NewGuid();
            
            if (!empresa.FechaRegistro.HasValue) 
                empresa.FechaRegistro = DateTime.UtcNow;
                
            if (!empresa.Activa.HasValue)
                empresa.Activa = true;

            await base.AddAsync(empresa);
        }

        public async Task DeleteAsync(Guid id)
        {
            var empresa = await GetByIdAsync(id);
            if (empresa != null)
            {
                empresa.Activa = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}