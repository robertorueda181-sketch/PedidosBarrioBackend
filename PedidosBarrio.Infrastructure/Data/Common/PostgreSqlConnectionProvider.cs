using Npgsql;
using System.Data;
using System.Text.RegularExpressions;

namespace PedidosBarrio.Infrastructure.Data.Common
{
    /// <summary>
    /// Proveedor de conexión para PostgreSQL
    /// Convierte queries de SQL Server a PostgreSQL
    /// </summary>
    public class PostgreSqlConnectionProvider : IDbConnectionProvider
    {
        private readonly string _connectionString;

        public DatabaseProvider Provider => DatabaseProvider.PostgreSQL;

        public PostgreSqlConnectionProvider(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "La cadena de conexión no puede estar vacía");
            
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public string ConvertParameterName(string sqlServerParamName)
        {
            // PostgreSQL/Npgsql usa :nombreParametro, no @
            var paramName = sqlServerParamName.StartsWith("@") ? sqlServerParamName.Substring(1) : sqlServerParamName;
            return $":{paramName}";
        }

        public string ConvertQuery(string sqlServerQuery)
        {
            if (string.IsNullOrEmpty(sqlServerQuery))
                return sqlServerQuery;

            var query = sqlServerQuery;

            // Convertir @nombreParametro a :nombreParametro
            query = Regex.Replace(query, @"@(\w+)", ":$1");

            // Convertir [dbo].[NombreTabla] a public.nometabla
            query = Regex.Replace(query, @"\[dbo\]\.\[(\w+)\]", "public.\"${1}\"");
            query = Regex.Replace(query, @"\[(\w+)\]", "\"${1}\"");

            // Convertir NEWID() a gen_random_uuid()
            query = query.Replace("NEWID()", "gen_random_uuid()");

            // Convertir GETUTCDATE() a NOW()
            query = query.Replace("GETUTCDATE()", "NOW()");

            // Convertir CAST(x AS BIT) a CAST(x AS BOOLEAN)
            query = Regex.Replace(query, @"CAST\s*\(\s*([^\)]+)\s+AS\s+BIT\s*\)", "CAST($1 AS BOOLEAN)");

            // Convertir TOP n a LIMIT n
            query = Regex.Replace(query, @"TOP\s+(\d+)", "LIMIT $1", RegexOptions.IgnoreCase);

            // Convertir DATEADD a date_trunc o interval
            query = Regex.Replace(query, 
                @"DATEADD\s*\(\s*YEAR\s*,\s*([^,]+)\s*,\s*([^\)]+)\s*\)", 
                "($2 + INTERVAL '$1 years')", 
                RegexOptions.IgnoreCase);

            // Convertir DBNull.Value a NULL (en comentarios, pero lo dejamos igual en SQL)
            // Ya que Dapper maneja esto correctamente

            return query;
        }

        public string GetNewGuidCommand()
        {
            // PostgreSQL usa gen_random_uuid() (requiere extensión pgcrypto)
            return "gen_random_uuid()";
        }
    }
}
