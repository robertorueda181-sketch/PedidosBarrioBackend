using Dapper;
using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class SearchRepository : GenericRepository, ISearchRepository
    {
        private readonly PedidosBarrioDbContext _context;
        private readonly IImageProcessingService _imageProcessingService;

        public SearchRepository(
            IDbConnectionProvider connectionProvider, 
            PedidosBarrioDbContext context,
            IImageProcessingService imageProcessingService) : base(connectionProvider)
        {
            _context = context;
            _imageProcessingService = imageProcessingService;
        }

        public async Task<IEnumerable<SearchResult>> SearchAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Enumerable.Empty<SearchResult>();

            var lowerTerm = term.ToLower();
            var results = new List<SearchResult>();

            // ===== PASO 1: OBTENER SOLO EMPRESAS VÁLIDAS (VISIBLE, ACTIVO, APROBADO) =====
            var empresasValidas = await _context.Empresas
                .AsNoTracking()
                .Where(e => e.Activa == true && e.Visible && e.Aprobado)
                .Select(e => e.ID)
                .ToListAsync();

            if (!empresasValidas.Any())
                return results;

            var allTypes = await _context.Tipos.AsNoTracking().ToDictionaryAsync(t => t.TipoID);

            // ===== PASO 2: BUSCAR PRODUCTOS EN EMPRESAS VÁLIDAS =====
            var productos = await _context.Productos
                .AsNoTracking()
                .Where(p => p.Activa == true && 
                           p.Aprobado == true &&
                           empresasValidas.Contains(p.EmpresaID.Value) &&
                           (p.Nombre.ToLower().Contains(lowerTerm) || 
                            (p.Descripcion != null && p.Descripcion.ToLower().Contains(lowerTerm))))
                .Take(20)
                .ToListAsync();

            var urlNegocioMap = await _context.Negocios
                .AsNoTracking()
                .Where(n => n.EmpresaID.HasValue && empresasValidas.Contains(n.EmpresaID.Value))
                .Select(n => new { n.EmpresaID, n.Urlnegocio, n.Urlopcional })
                .ToListAsync();

            foreach (var p in productos)
            {
                var img = await _context.Imagenes
                    .AsNoTracking()
                    .Where(img => img.ExternalId == p.ProductoID && img.Type == "PRODUCT" && img.Activa)
                    .OrderBy(img => img.Order)
                    .Select(img => img.Urlimagen)
                    .FirstOrDefaultAsync();

                var negocioUrl = urlNegocioMap.FirstOrDefault(u => u.EmpresaID == p.EmpresaID);

                results.Add(new SearchResult
                {
                    Type = "PRODUCTO",
                    Id = p.ProductoID,
                    Title = p.Nombre,
                    Description = p.Descripcion ?? "",
                    ImageUrl = !string.IsNullOrEmpty(img) ? await _imageProcessingService.GetImageUrlAsync(img) : "",
                    Url = negocioUrl?.Urlopcional ?? negocioUrl?.Urlnegocio ?? ""
                });
            }

            // ===== PASO 3: BUSCAR NEGOCIOS EN EMPRESAS VÁLIDAS =====
            var negocios = await _context.Negocios
               .AsNoTracking()
               .Include(n => n.Tipos)
               .Where(n => n.EmpresaID.HasValue &&
                          empresasValidas.Contains(n.EmpresaID.Value) &&
                          ((n.Nombre != null && n.Nombre.ToLower().Contains(lowerTerm)) ||
                           (n.Descripcion != null && n.Descripcion.ToLower().Contains(lowerTerm)) ||
                           (n.Urlnegocio != null && n.Urlnegocio.ToLower().Contains(lowerTerm))))
               .Take(20)
               .ToListAsync();

            // ===== PASO 3: BUSCAR NEGOCIOS EN EMPRESAS VÁLIDAS =====
           
            foreach (var n in negocios)
            {
                var img = await _context.Imagenes
                    .AsNoTracking()
                    .Where(img => img.ExternalId == n.NegocioID && img.Type == "PROFILE" && img.Activa == true)
                    .OrderBy(img => img.Order)
                    .Select(img => img.Urlimagen)
                    .FirstOrDefaultAsync();

                results.Add(new SearchResult
                {
                    Type = "NEGOCIO",
                    Id = n.NegocioID,
                    Title = n.Nombre ?? n.Urlnegocio ?? "Negocio",
                    Description = n.Descripcion ?? "",
                    Location = "",
                    Category = n.Tipos?.Descripcion ?? "Comercio",
                    ImageUrl = !string.IsNullOrEmpty(img) ? await _imageProcessingService.GetImageUrlAsync(img) : "",
                    Url = $"{n.Urlopcional ?? n.Urlnegocio}"
                });
            }

            // ===== PASO 4: BUSCAR INMUEBLES EN EMPRESAS VÁLIDAS =====
            var inmuebles = await _context.Inmuebles
                .AsNoTracking()
                .Include(i => i.Tipos)
                .Where(prop => prop.Activa == true &&
                           empresasValidas.Contains(prop.EmpresaID.Value) &&
                           ((prop.Descripcion != null && prop.Descripcion.ToLower().Contains(lowerTerm)) || 
                            (prop.Ubicacion != null && prop.Ubicacion.ToLower().Contains(lowerTerm))))
                .Take(20)
                .ToListAsync();

            foreach (var prop in inmuebles)
            {
                var img = await _context.Imagenes
                    .AsNoTracking()
                    .Where(img => img.ExternalId == prop.InmuebleID && img.Type == "PRODUCT" && img.Activa == true)
                    .OrderBy(img => img.Order)
                    .Select(img => img.Urlimagen)
                    .FirstOrDefaultAsync();

                string operacionDesc = "";
                if (prop.OperacionID.HasValue && allTypes.TryGetValue((int)prop.OperacionID.Value, out var opType))
                {
                    operacionDesc = opType.Descripcion ?? "";
                }

                results.Add(new SearchResult
                {
                    Type = "INMUEBLE",
                    Id = prop.InmuebleID,
                    Title = prop.Tipos?.Descripcion ?? "Inmueble",
                    Description = prop.Descripcion ?? "",
                    Location = prop.Ubicacion ?? "",
                    Category = prop.Tipos?.Descripcion ?? "Inmueble",
                    ImageUrl = !string.IsNullOrEmpty(img) ? await _imageProcessingService.GetImageUrlAsync(img) : "",
                    Price = prop.Precio,
                    Operacion = operacionDesc,
                    Medidas = prop.Medidas,
                    Dormitorios = prop.Dormitorios,
                    Banos = prop.Banos,
                    Url = $"/inmueble/{prop.InmuebleID}"
                });
            }

            return results;
        }
    }
}
