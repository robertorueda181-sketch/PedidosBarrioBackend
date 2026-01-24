using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities
{
    [Table("Ia_moderacion_logs")]
    public partial class IaModeracionLog
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Evaluacion")]
        public string Evaluacion { get; set; } = null!;

        [Column("EsTexto")]
        public bool EsTexto { get; set; }

        [Column("Apropiado")]
        public bool Apropiado { get; set; }

        [Column("FechaRegistro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Column("EmpresaID")]
        public Guid EmpresaID { get; set; }
    }
}
