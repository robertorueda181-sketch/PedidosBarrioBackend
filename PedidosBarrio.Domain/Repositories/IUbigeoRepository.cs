using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Domain.Repositories
{
    public interface IUbigeoRepository
    {
        Task<IEnumerable<UbigeoPeruDepartment>> GetDepartmentsAsync();
        Task<IEnumerable<UbigeoPeruProvince>> GetProvincesByDepartmentAsync(string departmentId);
        Task<IEnumerable<UbigeoPeruDistrict>> GetDistrictsByProvinceAsync(string provinceId);
    }
}
