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

            // 1. BUSCAR EN PRODUCTOS
            var productos = await _context.Productos
                .AsNoTracking()
                .Where(p => p.Activa == true && 
                           (p.Nombre.ToLower().Contains(lowerTerm) || 
                            (p.Descripcion != null && p.Descripcion.ToLower().Contains(lowerTerm))))
                .Take(20)
                .ToListAsync();

            foreach (var p in productos)
            {
                var img = await _context.Imagenes
                    .AsNoTracking()
                    .Where(img => img.ExternalId == p.ProductoID && img.Type == "PRODUCT" && img.Activa)
                    .OrderBy(img => img.Order)
                    .Select(img => img.Urlimagen)
                    .FirstOrDefaultAsync();

                results.Add(new SearchResult
                {
                    Type = "PRODUCTO",
                    Id = p.ProductoID,
                    Title = p.Nombre,
                    Description = p.Descripcion ?? "",
                    ImageUrl = !string.IsNullOrEmpty(img) ? await _imageProcessingService.GetImageUrlAsync(img) : "",
                    Url = $"/producto/{p.ProductoID}"
                });
            }

            // 2. BUSCAR EN NEGOCIOS
            var negocios = await _context.Negocios
                .AsNoTracking()
                .Include(n => n.Tipos)
                .Where(n => (n.Activa ?? true) && 
                           ((n.Nombre != null && n.Nombre.ToLower().Contains(lowerTerm)) || 
                            (n.Descripcion != null && n.Descripcion.ToLower().Contains(lowerTerm)) ||
                            (n.Urlnegocio != null && n.Urlnegocio.ToLower().Contains(lowerTerm))))
                .Take(20)
                .ToListAsync();

            foreach (var n in negocios)
            {
                var img = await _context.Imagenes
                    .AsNoTracking()
                    .Where(img => img.ExternalId == n.NegocioID && img.Type == "NEG" && img.Activa == true)
                    .OrderBy(img => img.Order)
                    .Select(img => img.Urlimagen)
                    .FirstOrDefaultAsync();

                results.Add(new SearchResult
                {
                    Type = "NEGOCIO",
                    Id = n.NegocioID,
                    Title = n.Nombre ?? n.Urlnegocio ?? "Negocio",
                    Description = n.Descripcion ?? "",
                    Location = n.Direccion ?? "",
                    Category = n.Tipos?.Descripcion ?? "Comercio",
                    ImageUrl = !string.IsNullOrEmpty(img) ? await _imageProcessingService.GetImageUrlAsync(img) : "",
                    Url = $"/negocio/{n.Urlnegocio ?? n.NegocioID.ToString()}"
                });
            }

            // 3. BUSCAR EN INMUEBLES (SERVICIOS/PROPIEDADES)
            var allTypes = await _context.Tipos.AsNoTracking().ToDictionaryAsync(t => t.TipoID);
            var inmuebles = await _context.Inmuebles
                .AsNoTracking()
                .Include(i => i.Tipos)
                .Where(prop => (prop.Activa ?? true) && 
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
