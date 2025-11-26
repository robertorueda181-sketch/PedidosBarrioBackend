using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllEmpresasAsync();
        Task<CompanyDto> GetEmpresaByIdAsync(Guid id);

        Task<CompanyDto> CreateEmpresaAsync(CreateCompanyDto createDto);

        Task UpdateEmpresaAsync(Guid id, CreateCompanyDto updateDto);
        Task DeleteEmpresaAsync(Guid id);
    }
}
