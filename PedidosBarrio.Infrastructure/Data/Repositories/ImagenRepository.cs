using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class ImagenRepository : IImagenRepository
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        public ImagenRepository(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<Imagen> GetByIdAsync(int id)
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            
            const string sql = @"
                SELECT ""ImagenID"", ""ExternalId"", ""URLImagen"", ""Descripcion"", 
                       ""FechaRegistro"", ""Activa"", ""Type"", ""order"" as ""Order"", ""EmpresaID""
                FROM ""Imagenes"" 
                WHERE ""ImagenID"" = @Id AND ""Activa"" = true";
            
            return await connection.QueryFirstOrDefaultAsync<Imagen>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Imagen>> GetAllAsync()
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            
            const string sql = @"
                SELECT ""ImagenID"", ""ExternalId"", ""URLImagen"", ""Descripcion"", 
                       ""FechaRegistro"", ""Activa"", ""Type"", ""order"" as ""Order"", ""EmpresaID""
                FROM ""Imagenes"" 
                WHERE ""Activa"" = true
                ORDER BY ""FechaRegistro"" DESC";
            
            return await connection.QueryAsync<Imagen>(sql);
        }

        public async Task<IEnumerable<Imagen>> GetByProductoIdAsync(int productoId, string tipo = "PRODUCT")
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            
            const string sql = @"
                SELECT ""ImagenID"", ""ExternalId"", ""URLImagen"", ""Descripcion"", 
                       ""FechaRegistro"", ""Activa"", ""Type"", ""order"" as ""Order"", ""EmpresaID""
                FROM ""Imagenes"" 
                WHERE ""ExternalId"" = @ProductoId AND ""Type"" = @Tipo AND ""Activa"" = true
                ORDER BY ""order"", ""ImagenID""";
            
            return await connection.QueryAsync<Imagen>(sql, new { ProductoId = productoId, Tipo = tipo });
        }

        public async Task<IEnumerable<Imagen>> GetByEmpresaIdAsync(Guid empresaId)
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            
            const string sql = @"
                SELECT ""ImagenID"", ""ExternalId"", ""URLImagen"", ""Descripcion"", 
                       ""FechaRegistro"", ""Activa"", ""Type"", ""order"" as ""Order"", ""EmpresaID""
                FROM ""Imagenes"" 
                WHERE ""EmpresaID"" = @EmpresaId AND ""Activa"" = true
                ORDER BY ""order"", ""ImagenID""";
            
            return await connection.QueryAsync<Imagen>(sql, new { EmpresaId = empresaId });
        }

        public async Task<Imagen> GetPrincipalByProductoIdAsync(int productoId)
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            
            const string sql = @"
                SELECT ""ImagenID"", ""ExternalId"", ""URLImagen"", ""Descripcion"", 
                       ""FechaRegistro"", ""Activa"", ""Type"", ""order"" as ""Order"", ""EmpresaID""
                FROM ""Imagenes"" 
                WHERE ""ExternalId"" = @ProductoId AND ""Type"" = 'PRODUCT' AND ""Activa"" = true
                ORDER BY ""order"", ""ImagenID""
                LIMIT 1";
            
            return await connection.QueryFirstOrDefaultAsync<Imagen>(sql, new { ProductoId = productoId });
        }

        public async Task<int> AddAsync(Imagen imagen)
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            
            const string sql = @"
                INSERT INTO ""Imagenes"" (""ExternalId"", ""URLImagen"", ""Descripcion"", 
                                        ""Type"", ""order"", ""EmpresaID"", ""Activa"")
                VALUES (@ExternalId, @URLImagen, @Descripcion, @Type, @Order, @EmpresaID, @Activa)
                RETURNING ""ImagenID""";
            
            var imagenId = await connection.QuerySingleAsync<int>(sql, imagen);
            imagen.ImagenID = imagenId;
            return imagenId;
        }

        public async Task UpdateAsync(Imagen imagen)
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            
            const string sql = @"
                UPDATE ""Imagenes"" 
                SET ""URLImagen"" = @URLImagen,
                    ""Descripcion"" = @Descripcion,
                    ""order"" = @Order
                WHERE ""ImagenID"" = @ImagenID AND ""EmpresaID"" = @EmpresaID";
            
            await connection.ExecuteAsync(sql, imagen);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            
            // Soft delete - cambiar Activa a false
            const string sql = @"
                UPDATE ""Imagenes"" 
                SET ""Activa"" = false 
                WHERE ""ImagenID"" = @Id";
            
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task SetPrincipalAsync(int imagenId, int productoId, Guid empresaId)
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            
            // Primero resetear todas las imágenes del producto para que no sean principales
            const string resetSql = @"
                UPDATE ""Imagenes"" 
                SET ""order"" = 2 
                WHERE ""ExternalId"" = @ProductoId AND ""EmpresaID"" = @EmpresaId AND ""Type"" = 'PRODUCT'";
            
            await connection.ExecuteAsync(resetSql, new { ProductoId = productoId, EmpresaId = empresaId });
            
            // Luego establecer la imagen seleccionada como principal
            const string setPrincipalSql = @"
                UPDATE ""Imagenes"" 
                SET ""order"" = 1 
                WHERE ""ImagenID"" = @ImagenId AND ""ExternalId"" = @ProductoId AND ""EmpresaID"" = @EmpresaId";
            
            await connection.ExecuteAsync(setPrincipalSql, new { ImagenId = imagenId, ProductoId = productoId, EmpresaId = empresaId });
        }
    }
}