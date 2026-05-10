using System.ComponentModel.DataAnnotations;

namespace PedidosBarrio.Application.DTOs;

// DTO para crear formulario de contacto / reserva
public class CreateFormularioContactoDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    [StringLength(150, ErrorMessage = "El email no puede exceder los 150 caracteres")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "La fecha de reserva es requerida")]
    public DateTime FechaReserva { get; set; }

    public TimeSpan? HoraReserva { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El número de personas debe ser mayor a 0")]
    public int? NumeroPersonas { get; set; } = 1;

    [StringLength(255, ErrorMessage = "La ocasión no puede exceder los 255 caracteres")]
    public string? Ocasion { get; set; }

    public string? Comentarios { get; set; }

    [Required(ErrorMessage = "El Codigo es requerida")]
    public string Codigo { get; set; } = string.Empty;
}

// DTO de respuesta
public class FormularioContactoResponseDto
{
    public Guid FormularioContactoID { get; set; }

    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public Guid? EmpresaID { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime? FechaReserva { get; set; }

    public TimeSpan? HoraReserva { get; set; }

    public int? NumeroPersonas { get; set; }

    public string? Ocasion { get; set; }

    public string? Comentarios { get; set; }
}

// DTO para listar formularios
public class FormularioContactoListDto
{
    public long FormularioContactoID { get; set; }

    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public Guid? EmpresaID { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool Activa { get; set; }

    public DateTime? FechaReserva { get; set; }

    public TimeSpan? HoraReserva { get; set; }

    public int? NumeroPersonas { get; set; }

    public string? Ocasion { get; set; }

    public string? Comentarios { get; set; }
}