using MediatR;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateNotificacionApp
{
    public class CreateNotificacionAppCommandHandler : IRequestHandler<CreateNotificacionAppCommand, int>
    {
        private readonly INotificacionAppRepository _repository;
        private readonly IApplicationLogger _logger;

        public CreateNotificacionAppCommandHandler(INotificacionAppRepository repository, IApplicationLogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<int> Handle(CreateNotificacionAppCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notificacion = new NotificacionApp
                {
                    EmpresaCodigo = request.EmpresaCodigo,
                    Mensaje = request.Mensaje,
                    FechaRegistro = DateTime.UtcNow,
                    Leida = false
                };

                var id = await _repository.AddAsync(notificacion);
                await _logger.LogInformationAsync($"Notificación app creada para empresa {request.EmpresaCodigo}", "CreateNotificacionAppCommand");
                return id;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error al crear notificación app: {ex.Message}", ex, "CreateNotificacionAppCommand");
                throw;
            }
        }
    }
}
