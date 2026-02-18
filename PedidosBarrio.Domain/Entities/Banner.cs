using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities
{
    [Table("Banner")]
    public class Banner
    {
        [Key]
        [Column("BannerID")]
        public int BannerID { get; set; }

        [Column("Titulo")]
        [StringLength(50)]
        public string? Titulo { get; set; }

        [Column("Descripcion")]
        [StringLength(150)]
        public string? Descripcion { get; set; }


        [Column("Link")]
        [StringLength(500)]
        public string? Link { get; set; }

        [Column("UrlImagen")]
        [StringLength(500)]
        public string? UrlImagen { get; set; }

        [Column("TextoBoton")]
        [StringLength(50)]
        public string? TextoBoton { get; set; }

        [Column("FechaInicio")]
        public DateTime FechaInicio { get; set; }


        [Column("FechaExpiracion")]
        public DateTime FechaExpiracion { get; set; }

        [Column("Visible")]
        public bool? Visible { get; set; }

        [Column("Aprobado")]
        public bool? Aprobado { get; set; }

        [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("EmpresaID")]
        public Guid EmpresaID { get; set; }

        [Column("Prioridad")]
        public short Prioridad { get; set; } = 1;
    }
}
