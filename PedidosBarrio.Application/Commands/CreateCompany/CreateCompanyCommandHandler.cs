using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PedidosBarrio.Application.Commands.CreateEmpresa;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreateCompany
{
    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyDto> // EmpresaDto -> CompanyDto
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCompanyDto> _createCompanyDtoValidator; // CreateEmpresaDto -> CreateCompanyDto

        public CreateCompanyCommandHandler(ICompanyRepository companyRepository, IMapper mapper, IValidator<CreateCompanyDto> createCompanyDtoValidator)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
            _createCompanyDtoValidator = createCompanyDtoValidator;
        }

        public async Task<CompanyDto> Handle(CreateCompanyCommand command, CancellationToken cancellationToken) // EmpresaDto -> CompanyDto
        {
            var createDto = new CreateCompanyDto // CreateEmpresaDto -> CreateCompanyDto
            {
                Name = command.Name, // Nombre -> Name
                Ruc = command.Ruc,
                PhoneNumber = command.PhoneNumber, // Celular -> PhoneNumber
                AddressStreet = command.AddressStreet,
                AddressCity = command.AddressCity,
                AddressZipCode = command.AddressZipCode
            };

            ValidationResult validationResult = await _createCompanyDtoValidator.ValidateAsync(createDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            var company = _mapper.Map<Company>(command); // Empresa -> Company
  
            await _companyRepository.AddAsync(company);
            return _mapper.Map<CompanyDto>(company); // EmpresaDto -> CompanyDto
        }
    }
}