using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using System.Data;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class TipoRepository : GenericRepository, ITipoRepository
    {
        public TipoRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<IEnumerable<Tipo>> GetByCategoriaAsync(string categoria, string param)
        {
            using (var connection = CreateConnection())
            {
                return await QueryAsync<Tipo>(
                    connection,
                    "sp_GetTiposByCategoria",
                    new { Tipo = categoria, param = param },
                    CommandType.StoredProcedure);
            }
        }
    }
}
