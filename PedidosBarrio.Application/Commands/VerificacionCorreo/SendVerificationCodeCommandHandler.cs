using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.VerificacionCorreo
{
    public class SendVerificationCodeCommandHandler : IRequestHandler<SendVerificationCodeCommand, bool>
    {
        private readonly IVerificarCorreoRepository _repository;
        private readonly IEmailService _emailService;

        public SendVerificationCodeCommandHandler(IVerificarCorreoRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public async Task<bool> Handle(SendVerificationCodeCommand request, CancellationToken cancellationToken)
        {
            // Generar código de 6 números
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            // Guardar en base de datos
            var verif = new VerificarCorreo
            {
                Correo = request.Correo,
                CodigoVerif = code,
                FechaCreacion = DateTime.UtcNow,
                FechaVecimiento = DateTime.UtcNow.AddMinutes(15)
            };

            await _repository.AddAsync(verif);

            // Obtener template de correo
            var template = await _emailService.GetTemplateAsync("VerificationCode");
            string emailBody;

            if (!string.IsNullOrEmpty(template))
            {
                emailBody = template
                    .Replace("{{CODE}}", code)
                    .Replace("{{YEAR}}", DateTime.Now.Year.ToString());
            }
            else
            {
                // Fallback en caso de que no se encuentre el archivo
                emailBody = $"Su código de verificación es: {code}";
            }

            // Enviar correo
            var emailRequest = new EmailRequestDto
            {
                To = request.Correo,
                ToName = "Usuario",
                Subject = "🔐 Código de verificación - Espacio Online",
                Body = emailBody,
                IsHtml = true
            };

            return await _emailService.SendEmailAsync(emailRequest);
        }
    }
}
