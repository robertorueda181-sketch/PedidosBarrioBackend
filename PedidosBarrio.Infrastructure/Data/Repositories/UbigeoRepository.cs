using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories
{
    public class UbigeoRepository : IUbigeoRepository
    {
        private readonly PedidosBarrioDbContext _context;

        public UbigeoRepository(PedidosBarrioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UbigeoPeruDepartment>> GetDepartmentsAsync()
        {
            return await _context.UbigeoPeruDepartments
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<UbigeoPeruProvince>> GetProvincesByDepartmentAsync(string departmentId)
        {
            return await _context.UbigeoPeruProvinces
                .Where(p => p.DepartmentId == departmentId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<UbigeoPeruDistrict>> GetDistrictsByProvinceAsync(string provinceId)
        {
            return await _context.UbigeoPeruDistricts
                .Where(d => d.ProvinceId == provinceId)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
    }
}
