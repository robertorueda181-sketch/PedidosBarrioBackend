using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    /// <summary>
    /// ProductoRepository usando Entity Framework Core
    /// Migrado completamente desde Dapper para mejor productividad y seguridad
    /// </summary>
    public class ProductoRepository : IProductoRepository
    {
        private readonly PedidosBarrioDbContext _context;

        public ProductoRepository(PedidosBarrioDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Producto> GetByIdAsync(int id, Guid empresaId)
        {
            var producto = await _context.Productos
                .Include(p => p.Presentaciones.Where(pr => pr.EmpresaID == empresaId))
                    .ThenInclude(pr => pr.Precios.Where(pre => pre.EmpresaID == empresaId))
                .FirstOrDefaultAsync(p => p.ProductoID == id && p.EmpresaID == empresaId && (p.Activa == true));

            if (producto == null)
            {
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
            }

            return producto;
        }

        public async Task<IEnumerable<Producto>> GetByEmpresaIdAsync(Guid empresaId)
        {
            return await _context.Productos
                .Where(p => p.EmpresaID == empresaId && (p.Activa == true))
                .Include(p => p.Presentaciones.Where(pr => pr.EmpresaID == empresaId))
                    .ThenInclude(pr => pr.Precios.Where(pre => pre.EmpresaID == empresaId))
                .OrderByDescending(p => p.FechaRegistro)
                .ToListAsync();
        }

        public async Task<int> AddAsync(Producto producto)
        {
            // Validar que no existe otro producto con el mismo nombre en la empresa
            var exists = await _context.Productos
                .AnyAsync(p => p.EmpresaID == producto.EmpresaID && 
                              p.Nombre.ToLower() == producto.Nombre.ToLower() && 
                              (p.Activa == true));

            if (exists)
            {
                throw new InvalidOperationException($"Ya existe un producto con el nombre '{producto.Nombre}' en esta empresa");
            }

            // Configurar valores por defecto
            producto.FechaRegistro = DateTime.UtcNow;
            producto.Activa = true;
            producto.Visible = true;

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return producto.ProductoID;
        }

        public async Task UpdateAsync(Producto producto)
        {
            var existing = await _context.Productos
                .FirstOrDefaultAsync(p => p.ProductoID == producto.ProductoID);

            if (existing == null)
            {
                throw new KeyNotFoundException($"Producto con ID {producto.ProductoID} no encontrado");
            }

            // Verificar que pertenece a la misma empresa (seguridad)
            if (existing.EmpresaID != producto.EmpresaID)
            {
                throw new UnauthorizedAccessException("No tiene permisos para modificar este producto");
            }

            // Actualizar solo los campos permitidos
            existing.Nombre = producto.Nombre;
            existing.Descripcion = producto.Descripcion;
            existing.StockMinimo = producto.StockMinimo;
            existing.Inventario = producto.Inventario;
            existing.Activa = producto.Activa;
            existing.Visible = producto.Visible;

            // Mantener campos que no se deben modificar en update
            // existing.EmpresaID - No cambiar
            // existing.FechaRegistro - No cambiar  
            // existing.Stock - Se actualiza por separado
            // existing.CategoriaID - Se actualiza por separado si es necesario

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            
            if (producto == null)
            {
                throw new KeyNotFoundException($"Producto con ID {id} no encontrado");
            }

            // Soft delete - mantener en BD pero marcado como inactivo
            producto.Activa = false;
            producto.Visible = false;
            
            await _context.SaveChangesAsync();
        }

        // ===== MÉTODOS ADICIONALES CON ENTITY FRAMEWORK =====

        /// <summary>
        /// Obtener productos con información completa incluyendo categoría
        /// </summary>
        public async Task<IEnumerable<Producto>> GetProductosConCategoriaAsync(Guid empresaId)
        {
            return await _context.Productos
                .Where(p => p.EmpresaID == empresaId && p.Activa == true && p.Visible == true)
                .Include(p => p.Presentaciones.Where(pr => pr.EmpresaID == empresaId))
                    .ThenInclude(pr => pr.Precios.Where(pre => pre.EmpresaID == empresaId))
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        /// <summary>
        /// Buscar productos por nombre o descripción
        /// </summary>
        public async Task<IEnumerable<Producto>> SearchProductosAsync(Guid empresaId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetByEmpresaIdAsync(empresaId);
            }

            var term = searchTerm.ToLower().Trim();
            
            return await _context.Productos
                .Where(p => p.EmpresaID == empresaId && 
                           p.Activa == true && 
                           p.Visible == true &&
                                       (p.Nombre.ToLower().Contains(term) || 
                                        p.Descripcion.ToLower().Contains(term)))
                           .Include(p => p.Presentaciones.Where(pr => pr.EmpresaID == empresaId))
                               .ThenInclude(pr => pr.Precios.Where(pre => pre.EmpresaID == empresaId))
                           .OrderBy(p => p.Nombre)
                           .ToListAsync();
        }

        /// <summary>
        /// Obtener productos por categoría
        /// </summary>
        public async Task<IEnumerable<Producto>> GetProductosByCategoriaAsync(Guid empresaId, short categoriaId)
        {
            return await _context.Productos
                .Where(p => p.EmpresaID == empresaId && 
                                      p.CategoriaID == categoriaId && 
                                      p.Activa == true && 
                                      p.Visible == true)
                           .Include(p => p.Presentaciones.Where(pr => pr.EmpresaID == empresaId))
                               .ThenInclude(pr => pr.Precios.Where(pre => pre.EmpresaID == empresaId))
                           .OrderBy(p => p.Nombre)
                           .ToListAsync();
        }

        /// <summary>
        /// Obtener productos con stock bajo
        /// </summary>
        public async Task<IEnumerable<Producto>> GetProductosConStockBajoAsync(Guid empresaId)
        {
            return await _context.Productos
                .Where(p => p.EmpresaID == empresaId && 
                           p.Activa == true && 
                           p.Inventario == true && 
                           p.Stock <= (p.StockMinimo ?? 0) &&
                           p.StockMinimo.HasValue)
                .OrderBy(p => p.Stock)
                .ThenBy(p => p.Nombre)
                .ToListAsync();
        }

        /// <summary>
        /// Actualizar solo el stock de un producto
        /// </summary>
        public async Task ActualizarStockAsync(int productoId, int nuevoStock, Guid empresaId)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.ProductoID == productoId && p.EmpresaID == empresaId);

            if (producto == null)
            {
                throw new KeyNotFoundException($"Producto con ID {productoId} no encontrado para la empresa especificada");
            }

            if (nuevoStock < 0)
            {
                throw new ArgumentException("El stock no puede ser negativo", nameof(nuevoStock));
            }

            producto.Stock = nuevoStock;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Verificar si existe un producto con ese nombre en la empresa
        /// </summary>
        public async Task<bool> ExistsProductoAsync(Guid empresaId, string nombre, int? excludeProductoId = null)
        {
            var query = _context.Productos
                .Where(p => p.EmpresaID == empresaId && 
                           p.Nombre.ToLower() == nombre.ToLower() && 
                           p.Activa == true);

            if (excludeProductoId.HasValue)
            {
                query = query.Where(p => p.ProductoID != excludeProductoId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Contar productos por empresa
        /// </summary>
        public async Task<int> CountProductosByEmpresaAsync(Guid empresaId, bool soloActivos = true)
        {
            var query = _context.Productos.Where(p => p.EmpresaID == empresaId);
            
            if (soloActivos)
            {
                query = query.Where(p => p.Activa == true);
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// Cambiar visibilidad de productos en lote
        /// </summary>
        public async Task CambiarVisibilidadLoteAsync(Guid empresaId, List<int> productosIds, bool visible)
        {
            var productos = await _context.Productos
                .Where(p => p.EmpresaID == empresaId && productosIds.Contains(p.ProductoID))
                .ToListAsync();

            foreach (var producto in productos)
            {
                producto.Visible = visible;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtener producto con detalles completos incluyendo precios e imágenes
        /// </summary>
        public async Task<Producto?> GetProductoCompletoAsync(int productoId, Guid empresaId)
        {
            return await _context.Productos
                .Where(p => p.ProductoID == productoId && 
                           p.EmpresaID == empresaId && 
                           p.Activa == true)
                .Include(p => p.Presentaciones.Where(pr => pr.EmpresaID == empresaId))
                    .ThenInclude(pr => pr.Precios.Where(pre => pre.EmpresaID == empresaId))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtener estadísticas de productos por empresa
        /// </summary>
        public async Task<object> GetEstadisticasAsync(Guid empresaId)
        {
            var totalProductos = await _context.Productos
                .CountAsync(p => p.EmpresaID == empresaId && p.Activa == true);

            var productosVisibles = await _context.Productos
                .CountAsync(p => p.EmpresaID == empresaId && p.Activa == true && p.Visible == true);

            var productosConInventario = await _context.Productos
                .CountAsync(p => p.EmpresaID == empresaId && p.Activa == true && p.Inventario == true);

            var productosStockBajo = await _context.Productos
                .CountAsync(p => p.EmpresaID == empresaId && 
                               p.Activa == true && 
                               p.Inventario == true && 
                               p.Stock <= (p.StockMinimo ?? 0) &&
                               p.StockMinimo.HasValue);

            return new
            {
                TotalProductos = totalProductos,
                ProductosVisibles = productosVisibles,
                ProductosConInventario = productosConInventario,
                ProductosStockBajo = productosStockBajo,
                PorcentajeVisibles = totalProductos > 0 ? (productosVisibles * 100.0) / totalProductos : 0
            };
        }

        /// <summary>
        /// Actualizar categoría de un producto
        /// </summary>
        public async Task CambiarCategoriaAsync(int productoId, short nuevaCategoriaId, Guid empresaId)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.ProductoID == productoId && p.EmpresaID == empresaId);

            if (producto == null)
            {
                throw new KeyNotFoundException($"Producto con ID {productoId} no encontrado para la empresa especificada");
            }

            producto.CategoriaID = nuevaCategoriaId;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtener productos más vendidos (requiere tabla de ventas)
        /// </summary>
        public async Task<IEnumerable<Producto>> GetProductosMasVendidosAsync(Guid empresaId, int top = 10)
        {
            // Por ahora retorna productos ordenados por nombre
            // Cuando implementes ventas, puedes cambiar la lógica
            return await _context.Productos
                .Where(p => p.EmpresaID == empresaId && p.Activa == true && p.Visible == true)
                .Include(p => p.Presentaciones.Where(pr => pr.EmpresaID == empresaId))
                    .ThenInclude(pr => pr.Precios.Where(pre => pre.EmpresaID == empresaId))
                .OrderBy(p => p.Nombre)
                .Take(top)
                .ToListAsync();
        }
    }
}
