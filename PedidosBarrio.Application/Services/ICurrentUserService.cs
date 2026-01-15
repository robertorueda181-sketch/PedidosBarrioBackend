namespace PedidosBarrio.Application.Services
{
    public interface ICurrentUserService
    {
        Guid GetEmpresaId();
        Guid GetUsuarioId();
        string GetUserEmail();
        bool IsAuthenticated();
    }
}