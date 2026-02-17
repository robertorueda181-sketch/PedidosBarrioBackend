using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class NotificacionAppRepository : EfCoreRepository<NotificacionApp>, INotificacionAppRepository
    {
        private readonly PedidosBarrioDbContext _context;
        public NotificacionAppRepository(PedidosBarrioDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(NotificacionApp notificacion)
        {
            await _context.AddAsync(notificacion);
            await _context.SaveChangesAsync();
            return notificacion.Id;
        }
    }
}
