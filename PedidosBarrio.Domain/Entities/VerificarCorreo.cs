using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities
{
    public class VerificarCorreo
    {
        [Key]
        [Column("VerifID")]
        public int VerifID { get; set; }

        [Required]
        [Column("Correo")]
        public string Correo { get; set; }

        [Required]
        [StringLength(6)]
        [Column("CodigoVerif")]
        public string CodigoVerif { get; set; }

        [Required]
        [Column("FechaVecimiento")]
        public DateTime FechaVecimiento { get; set; }

        [Required]
        [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
