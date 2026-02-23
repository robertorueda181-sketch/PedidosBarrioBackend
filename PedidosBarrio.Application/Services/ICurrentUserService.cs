namespace PedidosBarrio.Application.Services
{
    public interface ICurrentUserService
    {
        Guid GetEmpresaId();
        Guid GetUsuarioId();
        Guid GetClienteId();
        string GetUserEmail();
        bool IsAuthenticated();
        bool GetPasosIniciales();
    }
}