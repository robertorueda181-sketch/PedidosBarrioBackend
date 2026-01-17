using FluentValidation;
using PedidosBarrio.Application.Commands.ValidateImage;

namespace PedidosBarrio.Application.Validator
{
    public class ValidateImageCommandValidator : AbstractValidator<ValidateImageCommand>
    {
        public ValidateImageCommandValidator()
        {
            // Debe tener al menos una fuente de imagen
            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.ImageUrl) || !string.IsNullOrEmpty(x.Base64Image))
                .WithMessage("Debe proporcionar ImageUrl o Base64Image");

            // Validar URL si está presente
            When(x => !string.IsNullOrEmpty(x.ImageUrl), () =>
            {
                RuleFor(x => x.ImageUrl)
                    .Must(BeValidUrl)
                    .WithMessage("ImageUrl debe ser una URL válida");
            });

            // Validar Base64 si está presente
            When(x => !string.IsNullOrEmpty(x.Base64Image), () =>
            {
                RuleFor(x => x.Base64Image)
                    .Must(BeValidBase64)
                    .WithMessage("Base64Image debe ser una cadena base64 válida");
            });

            // Validar nivel de tolerancia
            RuleFor(x => x.ToleranceLevel)
                .Must(level => string.IsNullOrEmpty(level) || 
                              new[] { "LOW", "MEDIUM", "HIGH" }.Contains(level.ToUpper()))
                .WithMessage("ToleranceLevel debe ser LOW, MEDIUM o HIGH");
        }

        private bool BeValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var validUrl) &&
                   (validUrl.Scheme == Uri.UriSchemeHttp || validUrl.Scheme == Uri.UriSchemeHttps);
        }

        private bool BeValidBase64(string base64)
        {
            try
            {
                Convert.FromBase64String(base64);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}