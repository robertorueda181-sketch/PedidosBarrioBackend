using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities
{
    [Table("ubigeo_peru_provinces")]
    public class UbigeoPeruProvince
    {
        [Key]
        [Column("id")]
        [StringLength(4)]
        public string Id { get; set; } = null!;

        [Column("name")]
        [StringLength(45)]
        public string Name { get; set; } = null!;

        [Column("department_id")]
        [StringLength(2)]
        public string DepartmentId { get; set; } = null!;

        [ForeignKey("DepartmentId")]
        public virtual UbigeoPeruDepartment Department { get; set; } = null!;

        public virtual ICollection<UbigeoPeruDistrict> Districts { get; set; } = new List<UbigeoPeruDistrict>();
    }
}
