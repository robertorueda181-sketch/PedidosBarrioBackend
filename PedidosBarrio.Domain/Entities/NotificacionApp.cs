using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities
{
    [Table("notificacionesApp")]
    public class NotificacionApp
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("EmpresaCodigo")]
        [StringLength(100)]
        public string EmpresaCodigo { get; set; } = null!;

        [Column("Mensaje")]
        public string Mensaje { get; set; } = null!;

        [Column("FechaRegistro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Column("Leida")]
        public bool Leida { get; set; } = false;
    }
}
