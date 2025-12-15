using Microsoft.Data.SqlClient;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Common
{
    /// <summary>
    /// Proveedor de conexión para SQL Server
    /// </summary>
    public class SqlServerConnectionProvider : IDbConnectionProvider
    {
        private readonly string _connectionString;

        public DatabaseProvider Provider => DatabaseProvider.SqlServer;

        public SqlServerConnectionProvider(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "La cadena de conexión no puede estar vacía");
            
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public string ConvertParameterName(string sqlServerParamName)
        {
            // SQL Server usa @nombreParametro, no necesita conversión
            return sqlServerParamName.StartsWith("@") ? sqlServerParamName : $"@{sqlServerParamName}";
        }

        public string ConvertQuery(string sqlServerQuery)
        {
            // SQL Server no necesita conversión de queries
            return sqlServerQuery;
        }

        public string GetNewGuidCommand()
        {
            // SQL Server usa NEWID()
            return "NEWID()";
        }
    }
}
