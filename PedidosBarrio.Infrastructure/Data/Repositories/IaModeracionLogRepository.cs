using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class IaModeracionLogRepository : EfCoreRepository<IaModeracionLog>, IIaModeracionLogRepository
    {
        public IaModeracionLogRepository(PedidosBarrioDbContext context) : base(context)
        {
        }
    }
}
