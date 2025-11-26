using AutoMapper;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Interfaces;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;


namespace PedidosBarrio.Application.Services
{
    public class  CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllEmpresasAsync()
        {
            var empresas = await _companyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyDto>>(empresas);
        }

        public async Task<CompanyDto> GetEmpresaByIdAsync(Guid id)
        {
            var empresa = await _companyRepository.GetByIdAsync(id);
            return _mapper.Map<CompanyDto>(empresa);
        }

        public async Task<CompanyDto> CreateEmpresaAsync(CreateCompanyDto createDto)
        {
            // Mapear DTO a la entidad de dominio y construir Value Objects
            var company = _mapper.Map<Company>(createDto);
            await _companyRepository.AddAsync(company);
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task UpdateEmpresaAsync(Guid id, CreateCompanyDto updateDto)
        {
            var empresa = await _companyRepository.GetByIdAsync(id);
            if (empresa == null)
            {
                throw new ApplicationException($"Empresa con ID {id} no encontrada.");
            }

            var nuevoCelular = updateDto.PhoneNumber;

            empresa.UpdateInformation(updateDto.Name, nuevoCelular, updateDto.AddressStreet, updateDto.AddressCity, updateDto.AddressZipCode);
            await _companyRepository.UpdateAsync(empresa);
        }

        public async Task DeleteEmpresaAsync(Guid id)
        {
            await _companyRepository.DeleteAsync(id);
        }
    }
}
