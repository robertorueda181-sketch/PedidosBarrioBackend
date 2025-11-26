
using FluentValidation;
using PedidosBarrio.Application.DTOs;


namespace PedidosBarrio.Application.Validator
{
    public class CreateCompanyDtoValidator : AbstractValidator<CreateCompanyDto>
    {
        public CreateCompanyDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotEmpty().WithMessage("El nombre de la empresa no puede estar vacío.")
                .MaximumLength(255).WithMessage("El nombre no puede exceder los 255 caracteres.");

            RuleFor(dto => dto.Ruc)
                .NotEmpty().WithMessage("El RUC de la empresa es obligatorio.")
                .Length(11).WithMessage("El RUC debe tener 11 dígitos.")
                .Must(BeNumeric).WithMessage("El RUC debe contener solo números.");

            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty().WithMessage("El celular de la empresa es obligatorio.")
                .Length(9, 15).WithMessage("El celular debe tener entre 9 y 15 dígitos.") 
                .Must(BeNumeric).WithMessage("El celular debe contener solo números.");

            RuleFor(dto => dto.AddressStreet)
                .NotEmpty().WithMessage("La calle de la dirección es obligatoria.")
                .MaximumLength(255).WithMessage("La calle no puede exceder los 255 caracteres.");

            RuleFor(dto => dto.AddressCity)
                .NotEmpty().WithMessage("La ciudad de la dirección es obligatoria.")
                .MaximumLength(100).WithMessage("La ciudad no puede exceder los 100 caracteres.");

            RuleFor(dto => dto.AddressZipCode)
                .MaximumLength(20).WithMessage("El código postal no puede exceder los 20 caracteres.");
            // Puedes agregar .NotEmpty() si es obligatorio
        }

        private bool BeNumeric(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return long.TryParse(value, out _);
        }
    }
}