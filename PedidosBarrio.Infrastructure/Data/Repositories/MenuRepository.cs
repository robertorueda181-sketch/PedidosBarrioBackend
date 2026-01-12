using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class MenuRepository : GenericRepository, IMenuRepository
    {
        public MenuRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public async Task<IEnumerable<MenuItem>> GetMenusByEmpresaAsync(Guid empresaId)
        {
            using (var connection = CreateConnection())
            {
                var sql = "SELECT * FROM public.fn_obtener_menus_por_empresa(@p_empresa_id)";
                return await QueryAsync<MenuItem>(connection, sql, new { p_empresa_id = empresaId }, CommandType.Text);
            }
        }
    }
}