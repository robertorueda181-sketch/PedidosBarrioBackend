using MediatR;

namespace PedidosBarrio.Application.Commands.RegisterPageView;

/// <summary>
/// Comando para registrar una visita de página
/// </summary>
public class RegisterPageViewCommand : IRequest<bool>
{
    /// <summary>
    /// Código de la empresa (ej: "EMPRESA-001")
    /// El handler resuelve esto a EmpresaID internamente
    /// </summary>
    public string CodigoEmpresa { get; set; }

    public string Url { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
        public string? Referrer { get; set; }

        public RegisterPageViewCommand(
            string codigoEmpresa,
            string url,
            string? userAgent = null,
            string? ipAddress = null,
            string? referrer = null)
        {
            CodigoEmpresa = codigoEmpresa;
            Url = url;
            UserAgent = userAgent;
            IpAddress = ipAddress;
            Referrer = referrer;
        }
    }
