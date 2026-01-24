using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class VerificarCorreoRepository : IVerificarCorreoRepository
    {
        private readonly PedidosBarrioDbContext _context;

        public VerificarCorreoRepository(PedidosBarrioDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(VerificarCorreo verif)
        {
            _context.VerificarCorreos.Add(verif);
            await _context.SaveChangesAsync();
        }

        public async Task<VerificarCorreo> GetValidCodeAsync(string correo, string codigo)
        {
            var now = DateTime.UtcNow;
            return await _context.VerificarCorreos
                .Where(v => v.Correo == correo && 
                            v.CodigoVerif == codigo && 
                            v.FechaVecimiento > now)
                .OrderByDescending(v => v.FechaCreacion)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(VerificarCorreo verif)
        {
            _context.VerificarCorreos.Remove(verif);
            await _context.SaveChangesAsync();
        }
    }
}
