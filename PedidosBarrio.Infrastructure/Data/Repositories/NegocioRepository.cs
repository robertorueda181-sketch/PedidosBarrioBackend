using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;
using Microsoft.Extensions.Configuration;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class NegocioRepository : EfCoreRepository<Negocio>, INegocioRepository
    {
        private readonly string _baseUrl;
        private readonly string _negocioUrl = "negocio";

        public NegocioRepository(PedidosBarrioDbContext context, IConfiguration configuration) : base(context)
        {
            var baseUrl = configuration["BaseUrlFront"] ?? "";
            _baseUrl = baseUrl.TrimEnd('/');
        }

        private void SetFullUrl(Negocio? negocio)
        {
            if (negocio == null) return;
            if (!string.IsNullOrEmpty(negocio.Urlnegocio) && !negocio.Urlnegocio.StartsWith("http"))
            {
                negocio.Urlnegocio = $"{_baseUrl}/{_negocioUrl}/{negocio.Urlnegocio.TrimStart('/')}";
                negocio.BaseUrl = _baseUrl;
            }
        }

        public async Task<Negocio?> GetByIdAsync(string id)
        {
            Negocio? negocio;
            if (int.TryParse(id, out int negocioId))
            {
                negocio = await _context.Negocios
                    .AsNoTracking()
                    .Where(n => n.Empresa != null && 
                                n.Empresa.Activa == true && 
                                n.Empresa.Visible && 
                                n.Empresa.Aprobado)
                    .Include(n => n.Tipos)
                    .Include(n => n.Empresa)
                    .FirstOrDefaultAsync(n => n.NegocioID == negocioId);
            }
            else
            {
                negocio = await _context.Negocios
                    .AsNoTracking()
                    .Where(n => n.Empresa != null && 
                                n.Empresa.Activa == true && 
                                n.Empresa.Visible && 
                                n.Empresa.Aprobado)
                    .Include(n => n.Tipos)
                    .Include(n => n.Empresa)
                    .FirstOrDefaultAsync(n => n.Codigo == id || n.Urlnegocio == id);
            }

            SetFullUrl(negocio);
            return negocio;
        }

        public async Task<Empresa?> GetByCodigoEmpresaAsync(string id)
        {
            var negocio = await _context.Negocios
                .AsNoTracking()
                .Where(n => n.Empresa != null && 
                            n.Empresa.Activa == true && 
                            n.Empresa.Visible && 
                            n.Empresa.Aprobado)
                .Include(n => n.Empresa)
                .FirstOrDefaultAsync(n => n.Codigo == id || n.Urlnegocio == id);

            return negocio?.Empresa;
        }

        public new async Task<IEnumerable<Negocio>> GetAllAsync()
        {
            var negocios = await _context.Negocios
                .AsNoTracking()
                .Where(n => n.Empresa != null && 
                            n.Empresa.Activa == true && 
                            n.Empresa.Visible && 
                            n.Empresa.Aprobado)
                .Include(n => n.Tipos)
                .ToListAsync();

            foreach (var n in negocios) SetFullUrl(n);
            return negocios;
        }

        public async Task<IEnumerable<Negocio>> GetByEmpresaIdAsync(Guid empresaId)
        {
            var negocios = await _context.Negocios
                .AsNoTracking()
                .Where(n => n.EmpresaID == empresaId)
                .Include(n => n.Tipos)
                .ToListAsync();

            foreach (var n in negocios) SetFullUrl(n);
            return negocios;
        }

        public async Task<int> AddAsync(Negocio negocio)
        {
            SetFullUrl(negocio);
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






