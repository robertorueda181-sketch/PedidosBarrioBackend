using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

[Table("Direccion")]
public partial class Direccion
{
    public Direccion() { }

    public Direccion(Guid empresaId, string nombreLocal, string direccion, decimal longitud, decimal latitud)
    {
        EmpresaID = empresaId;
        NombreLocal = nombreLocal;
        DireccionTexto = direccion;
        Longitud = longitud;
        Latitud = latitud;
    }

    [Key]
    [Column("DireccionID")]
    public int DireccionID { get; set; }

    [Column("EmpresaID")]
    public Guid EmpresaID { get; set; }

    [Required]
    [Column("NombreLocal")]
    public string NombreLocal { get; set; } = null!;

    [Required]
    [Column("Direccion")]
    public string DireccionTexto { get; set; } = null!;

    public string? Referencia { get; set; }

    [Column(TypeName = "numeric(9,6)")]
    public decimal Longitud { get; set; }

    [Column(TypeName = "numeric(9,6)")]
    public decimal Latitud { get; set; }

    [StringLength(100)]
    public string? Departamento { get; set; }

    [StringLength(100)]
    public string? Provincia { get; set; }

    [StringLength(100)]
    public string? Distrito { get; set; }

    [ForeignKey("EmpresaID")]
    public virtual Empresa Empresa { get; set; } = null!;
}
