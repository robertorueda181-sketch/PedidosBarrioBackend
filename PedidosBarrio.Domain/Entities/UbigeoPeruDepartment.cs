using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities
{
    [Table("ubigeo_peru_departments")]
    public class UbigeoPeruDepartment
    {
        [Key]
        [Column("id")]
        [StringLength(2)]
        public string Id { get; set; } = null!;

        [Column("name")]
        [StringLength(45)]
        public string Name { get; set; } = null!;

        public virtual ICollection<UbigeoPeruProvince> Provinces { get; set; } = new List<UbigeoPeruProvince>();
    }
}
