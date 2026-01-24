using MediatR;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.VerificacionCorreo
{
    public class VerifyCodeCommandHandler : IRequestHandler<VerifyCodeCommand, bool>
    {
        private readonly IVerificarCorreoRepository _repository;

        public VerifyCodeCommandHandler(IVerificarCorreoRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
        {
            var verification = await _repository.GetValidCodeAsync(request.Correo, request.Codigo);

            if (verification == null)
            {
                return false;
            }

            // Opcional: Eliminar o marcar como usado para que no se use de nuevo
            // await _repository.DeleteAsync(verification);

            return true;
        }
    }
}
