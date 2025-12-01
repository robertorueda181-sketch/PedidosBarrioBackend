namespace PedidosBarrio.CrossCutting.Logging
{
    public interface ILogger
    {
        Task LogInformationAsync(string message, string category = "General");
        Task LogWarningAsync(string message, string category = "General");
        Task LogErrorAsync(string message, Exception exception = null, string category = "General");
        Task LogDebugAsync(string message, string category = "General");
    }
}
