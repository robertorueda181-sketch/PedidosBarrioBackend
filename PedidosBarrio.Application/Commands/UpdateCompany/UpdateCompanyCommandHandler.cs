using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.UpdateCompany
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Unit>
    {
        private readonly ICompanyRepository _companyRepository; // IEmpresaRepository -> ICompanyRepository
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCompanyDto> _createCompanyDtoValidator; // IValidator<CreateEmpresaDto> -> IValidator<CreateCompanyDto>

        public UpdateCompanyCommandHandler(ICompanyRepository companyRepository, IMapper mapper, IValidator<CreateCompanyDto> createCompanyDtoValidator)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
            _createCompanyDtoValidator = createCompanyDtoValidator;
        }

        public async Task<Unit> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
        {
            var updateDto = new CreateCompanyDto // CreateEmpresaDto -> CreateCompanyDto
            {
                Name = command.Name,
                Ruc = command.Ruc,
                PhoneNumber = command.PhoneNumber,
                AddressStreet = command.AddressStreet,
                AddressCity = command.AddressCity,
                AddressZipCode = command.AddressZipCode
            };

            ValidationResult validationResult = await _createCompanyDtoValidator.ValidateAsync(updateDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var company = await _companyRepository.GetByIdAsync(command.Id); // empresa -> company
            if (company == null)
            {
                throw new ApplicationException($"Company with ID {command.Id} not found."); // Empresa -> Company
            }

            company.UpdateInformation( // empresa -> company
                command.Name,
                command.PhoneNumber,
                command.AddressStreet,
                command.AddressCity,
                command.AddressZipCode
            );

            await _companyRepository.UpdateAsync(company); // empresa -> company
            return Unit.Value;
        }
    }
}