using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Common
{
    /// <summary>
    /// Abstracción para proveedores de conexión a base de datos
    /// Permite cambiar entre SQL Server, PostgreSQL, etc. sin cambiar los repositorios
    /// </summary>
    public interface IDbConnectionProvider
    {
        /// <summary>
        /// Obtiene el proveedor de base de datos
        /// </summary>
        DatabaseProvider Provider { get; }

        /// <summary>
        /// Crea una nueva conexión a la base de datos
        /// </summary>
        IDbConnection CreateConnection();

        /// <summary>
        /// Convierte parámetros SQL Server a SQL específico del proveedor
        /// </summary>
        string ConvertParameterName(string sqlServerParamName);

        /// <summary>
        /// Convierte una consulta SQL Server a SQL específico del proveedor
        /// </summary>
        string ConvertQuery(string sqlServerQuery);

        /// <summary>
        /// Obtiene el comando para generar un GUID
        /// </summary>
        string GetNewGuidCommand();
    }
}
