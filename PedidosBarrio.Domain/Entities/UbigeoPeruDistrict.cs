using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedidosBarrio.Domain.Entities
{
    [Table("ubigeo_peru_districts")]
    public class UbigeoPeruDistrict
    {
        [Key]
        [Column("id")]
        [StringLength(6)]
        public string Id { get; set; } = null!;

        [Column("name")]
        [StringLength(45)]
        public string? Name { get; set; }

        [Column("province_id")]
        [StringLength(4)]
        public string? ProvinceId { get; set; }

        [Column("department_id")]
        [StringLength(2)]
        public string? DepartmentId { get; set; }

        [ForeignKey("ProvinceId")]
        public virtual UbigeoPeruProvince? Province { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual UbigeoPeruDepartment? Department { get; set; }
    }
}
