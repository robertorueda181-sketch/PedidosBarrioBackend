namespace PedidosBarrio.Application.DTOs;

public class ClienteDireccionDto
{
    public Guid ClienteDireccionID { get; set; }
    public Guid ClienteID { get; set; }
    
    /// <summary>
    /// Nombre de la dirección (Casa, Trabajo, Departamento, etc.)
    /// </summary>
    public string Nombre { get; set; } = null!;
    
    /// <summary>
    /// Texto completo de la dirección
    /// </summary>
    public string DireccionTexto { get; set; } = null!;
    
    /// <summary>
    /// Referencia adicional
    /// </summary>
    public string? Referencia { get; set; }
    
    /// <summary>
    /// Coordenada de latitud
    /// </summary>
    public decimal Latitud { get; set; }
    
    /// <summary>
    /// Coordenada de longitud
    /// </summary>
    public decimal Longitud { get; set; }
    
    /// <summary>
    /// Ubicación administrativa
    /// </summary>
    public string? Departamento { get; set; }
    public string? Provincia { get; set; }
    public string? Distrito { get; set; }
    public string? CodigoPostal { get; set; }
    
    /// <summary>
    /// Indica si es la dirección principal
    /// </summary>
    public bool EsPrincipal { get; set; }
    
    /// <summary>
    /// Indica si está activa
    /// </summary>
    public bool Activa { get; set; }
    
    /// <summary>
    /// Fechas de auditoría
    /// </summary>
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}

public class CreateClienteDireccionDto
{
    /// <summary>
    /// Nombre de la dirección (Casa, Trabajo, etc.)
    /// </summary>
    public string Nombre { get; set; } = null!;
    
    /// <summary>
    /// Texto completo de la dirección
    /// </summary>
    public string DireccionTexto { get; set; } = null!;
    
    /// <summary>
    /// Referencia adicional (opcional)
    /// </summary>
    public string? Referencia { get; set; }
    
    /// <summary>
    /// Coordenadas geográficas
    /// </summary>
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    
    /// <summary>
    /// Información de ubicación administrativa (opcional)
    /// </summary>
    public string? Departamento { get; set; }
    public string? Provincia { get; set; }
    public string? Distrito { get; set; }
    public string? CodigoPostal { get; set; }
    
    /// <summary>
    /// Marcar como principal
    /// </summary>
    public bool EsPrincipal { get; set; } = false;
}

public class UpdateClienteDireccionDto
{
    public string? Nombre { get; set; }
    public string? DireccionTexto { get; set; }
    public string? Referencia { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
    public string? Departamento { get; set; }
    public string? Provincia { get; set; }
    public string? Distrito { get; set; }
    public string? CodigoPostal { get; set; }
    public bool? EsPrincipal { get; set; }
    public bool? Activa { get; set; }
}
