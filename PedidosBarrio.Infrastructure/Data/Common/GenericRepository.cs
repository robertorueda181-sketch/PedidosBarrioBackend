using Dapper;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Common
{
    /// <summary>
    /// Clase base para repositorios agnósticos de la base de datos
    /// Maneja la conversión automática de queries y parámetros
    /// </summary>
    public abstract class GenericRepository
    {
        protected readonly IDbConnectionProvider _connectionProvider;

        public DatabaseProvider DatabaseProvider => _connectionProvider.Provider;

        protected GenericRepository(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        /// <summary>
        /// Crea una conexión a la base de datos
        /// </summary>
        protected IDbConnection CreateConnection() => _connectionProvider.CreateConnection();

        /// <summary>
        /// Convierte un nombre de parámetro SQL Server al formato del proveedor actual
        /// </summary>
        protected string ConvertParamName(string paramName) => _connectionProvider.ConvertParameterName(paramName);

        /// <summary>
        /// Convierte una query SQL Server al SQL del proveedor actual
        /// </summary>
        protected string ConvertQuery(string query) => _connectionProvider.ConvertQuery(query);

        /// <summary>
        /// Ejecuta una query que devuelve un resultado único
        /// </summary>
        protected async Task<T> QuerySingleOrDefaultAsync<T>(
            IDbConnection connection, 
            string query, 
            object parameters = null, 
            CommandType commandType = CommandType.Text)
        {
            query = ConvertQuery(query);
            return await connection.QuerySingleOrDefaultAsync<T>(query, parameters, commandType: commandType);
        }

        /// <summary>
        /// Ejecuta una query que devuelve múltiples resultados
        /// </summary>
        protected async Task<IEnumerable<T>> QueryAsync<T>(
            IDbConnection connection, 
            string query, 
            object parameters = null, 
            CommandType commandType = CommandType.Text)
        {
            query = ConvertQuery(query);
            return await connection.QueryAsync<T>(query, parameters, commandType: commandType);
        }

        /// <summary>
        /// Ejecuta un comando (INSERT, UPDATE, DELETE)
        /// </summary>
        protected async Task<int> ExecuteAsync(
            IDbConnection connection, 
            string query, 
            object parameters = null, 
            CommandType commandType = CommandType.Text)
        {
            query = ConvertQuery(query);
            return await connection.ExecuteAsync(query, parameters, commandType: commandType);
        }

        /// <summary>
        /// Obtiene un comando SQL usando el proveedor actual
        /// Usa reflection para compatibilidad con stored procedures
        /// </summary>
        protected CommandDefinition GetCommandDefinition(string commandName, object parameters, bool isStoredProcedure = true)
        {
            if (isStoredProcedure)
            {
                // Para PostgreSQL: convertir stored procedure a función PL/pgSQL
                if (_connectionProvider.Provider == DatabaseProvider.PostgreSQL)
                {
                    // Convertir "sp_GetEmpresaById" a "SELECT * FROM sp_getemparesabyid(:empresaid)"
                    // Esto requiere que en PostgreSQL tengas funciones equivalentes
                    // Por ahora, retornamos con el nombre convertido
                }
            }

            return new CommandDefinition(commandName, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
