using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Queries.GetEmpresaById;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetCompanyById
{
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, CompanyDto> // EmpresaDto -> CompanyDto
    {
        private readonly ICompanyRepository _companyRepository; // IEmpresaRepository -> ICompanyRepository
        private readonly IMapper _mapper;

        public GetCompanyByIdQueryHandler(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<CompanyDto> Handle(GetCompanyByIdQuery query, CancellationToken cancellationToken) // EmpresaDto -> CompanyDto
        {
            var company = await _companyRepository.GetByIdAsync(query.Id); // empresa -> company
            if (company == null)
            {
                return null;
            }
            return _mapper.Map<CompanyDto>(company); // EmpresaDto -> CompanyDto
        }
    }
}