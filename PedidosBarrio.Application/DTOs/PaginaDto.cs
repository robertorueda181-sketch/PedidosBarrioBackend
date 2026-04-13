namespace PedidosBarrio.Application.DTOs;

/// <summary>
/// DTO para leer una página
/// </summary>
public class PaginaDto
{
    public Guid PaginaID { get; set; }
    public string Contenido { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activa { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
}

/// <summary>
/// DTO para crear una página
/// El código del negocio se obtiene automáticamente del token JWT
/// </summary>
public class CreatePaginaDto
{
    /// <summary>
    /// Contenido JSONB de la página
    /// </summary>
    public string Contenido { get; set; } = null!;

    /// <summary>
    /// Descripción opcional de la página
    /// </summary>
    public string? Descripcion { get; set; }
}

/// <summary>
/// DTO para actualizar una página
/// </summary>
public class UpdatePaginaDto
{
    public string? Contenido { get; set; }
    public string? Descripcion { get; set; }
    public bool? Activa { get; set; }
}

/// <summary>
/// DTO para respuesta de páginas
/// </summary>
public class PaginaResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public PaginaDto? Data { get; set; }
}

/// <summary>
/// DTO para respuesta de listado de páginas
/// </summary>
public class PaginasResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public IEnumerable<PaginaDto>? Data { get; set; }
}
