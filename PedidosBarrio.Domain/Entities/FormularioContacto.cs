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


    [StringLength(20)]
    [Column("Telefono")]
    public string? Telefono { get; set; }

    [Column("NumeroPersonas")]
    public string? NumeroPersonas { get; set; } = "1";

    [StringLength(255)]
    [Column("Ocasion")]
    public string? Ocasion { get; set; }

    [Column("Comentarios")]
    public string? Comentarios { get; set; }

    [StringLength(50)]
    [Column("EmpresaID")]
    public Guid? EmpresaID { get; set; }

    [Column("FechaReserva", TypeName = "timestamp without time zone")]
    public DateTime? FechaReserva { get; set; }

    [Column("HoraReserva", TypeName = "time")]
    public TimeSpan? HoraReserva { get; set; }

    [Column("FechaRegistro", TypeName = "timestamp with time zone")]
    public DateTime FechaRegistro { get; set; }

    [Column("Activa")]
    public bool Activa { get; set; }
}
