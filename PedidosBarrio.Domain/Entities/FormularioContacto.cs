using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

public class FormularioContacto
{
    [Column("Id")]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    [Column("Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [Column("Email")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    [Column("Telefono")]
    public string? Telefono { get; set; }

    [Column("FechaReserva")]
    public DateTime? FechaReserva { get; set; }

    [Column("HoraReserva")]
    public TimeSpan? HoraReserva { get; set; }

    [Column("NumeroPersonas")]
    public int? NumeroPersonas { get; set; } = 1;

    [StringLength(255)]
    [Column("Ocasion")]
    public string? Ocasion { get; set; }

    [Column("Comentarios")]
    public string? Comentarios { get; set; }

    [Required]
    [StringLength(200)]
    [Column("Asunto")]
    public string Asunto { get; set; } = string.Empty;

    [Required]
    [Column("Mensaje")]
    public string Mensaje { get; set; } = string.Empty;

    [StringLength(50)]
    [Column("EmpresaID")]
    public Guid? EmpresaID { get; set; }

    [Column("FechaRegistro")]
    public DateTime FechaRegistro { get; set; }

    [Column("Activa")]
    public bool Activa { get; set; }
}
