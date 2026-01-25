using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities;

public partial class Menu
{
    [Key]
    [Column("MenuID")]
    public short MenuID { get; set; }

    [StringLength(50)]
    public string? Nombre { get; set; }

    [Column("icon")]
    [StringLength(20)]
    public string? Icon { get; set; }

    [Column("codigo")]
    [StringLength(50)]
    public string? Codigo { get; set; }

    [Column("padre", TypeName = "character varying")]
    public string? Padre { get; set; }

    [Column("order")]
    public short Order { get; set; }

    public short? NivelSuscripcion { get; set; }
}


