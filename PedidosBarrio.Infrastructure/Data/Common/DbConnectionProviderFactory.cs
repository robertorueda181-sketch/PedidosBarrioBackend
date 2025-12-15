namespace PedidosBarrio.Infrastructure.Data.Common
{
    /// <summary>
    /// Factory para crear proveedores de conexión según la configuración
    /// </summary>
    public class DbConnectionProviderFactory
    {
        /// <summary>
        /// Crea un proveedor de conexión basado en el tipo especificado
        /// </summary>
        public static IDbConnectionProvider Create(DatabaseProvider provider, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "La cadena de conexión no puede estar vacía");

            return provider switch
            {
                DatabaseProvider.SqlServer => new SqlServerConnectionProvider(connectionString),
                DatabaseProvider.PostgreSQL => new PostgreSqlConnectionProvider(connectionString),
                _ => throw new NotSupportedException($"El proveedor de base de datos '{provider}' no está soportado")
            };
        }

        /// <summary>
        /// Crea un proveedor de conexión basado en una cadena de configuración
        /// </summary>
        public static IDbConnectionProvider CreateFromString(string providerString, string connectionString)
        {
            if (!Enum.TryParse<DatabaseProvider>(providerString, ignoreCase: true, out var provider))
                throw new ArgumentException($"Proveedor de base de datos desconocido: {providerString}");

            return Create(provider, connectionString);
        }
    }
}
