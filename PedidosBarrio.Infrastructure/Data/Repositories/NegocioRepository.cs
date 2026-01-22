using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class NegocioRepository : EfCoreRepository<Negocio>, INegocioRepository
    {
        public NegocioRepository(PedidosBarrioDbContext context) : base(context)
        {
        }

        public async Task<Negocio?> GetByIdAsync(string id)
        {
            if (int.TryParse(id, out int negocioId))
            {
                return await _context.Negocios
                    .Include(n => n.Tipos)
                    .FirstOrDefaultAsync(n => n.NegocioID == negocioId);
            }

            return await _context.Negocios
                .Include(n => n.Tipos)
                .FirstOrDefaultAsync(n => n.Codigo == id || n.Urlnegocio == id);
        }

        public async Task<Empresa?> GetByCodigoEmpresaAsync(string id)
        {
            var negocio = await _context.Negocios
                .Include(n => n.Empresa)
                .FirstOrDefaultAsync(n => n.Codigo == id || n.Urlnegocio == id);

            if (negocio?.Empresa != null && !string.IsNullOrEmpty(negocio.Referencia))
            {
                negocio.Empresa.Referencia = negocio.Referencia;
            }

            return negocio?.Empresa;
        }

        public new async Task<IEnumerable<Negocio>> GetAllAsync()
        {
            return await _context.Negocios
                .Include(n => n.Tipos)
                .ToListAsync();
        }

        public async Task<IEnumerable<Negocio>> GetByEmpresaIdAsync(Guid empresaId)
        {
            return await _context.Negocios
                .Where(n => n.EmpresaID == empresaId)
                .Include(n => n.Tipos)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Negocio negocio)
        {
            await base.AddAsync(negocio);
            return negocio.NegocioID;
        }

        public new async Task UpdateAsync(Negocio negocio)
        {
            var existing = await _context.Negocios.FindAsync(negocio.NegocioID);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(negocio);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var negocio = await _context.Negocios.FindAsync(id);
            if (negocio != null)
            {
                _context.Negocios.Remove(negocio);
                await _context.SaveChangesAsync();
            }
        }
    }
}



