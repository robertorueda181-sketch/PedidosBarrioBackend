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

            // Enviar correo
            var emailRequest = new EmailRequestDto
            {
                To = request.Correo,
                ToName = "Usuario",
                Subject = "Código de verificación - Espacio Online",
                Body = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px;'>
                        <h2 style='color: #007bff; text-align: center;'>Espacio Online</h2>
                        <p>Hola,</p>
                        <p>Has solicitado un código de verificación para tu cuenta. Por favor, utiliza el siguiente código para completar el proceso:</p>
                        <div style='background-color: #f8f9fa; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; border-radius: 5px; margin: 20px 0;'>
                            {code}
                        </div>
                        <p style='color: #666; font-size: 14px;'>Este código es válido por 15 minutos.</p>
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                        <p style='text-align: center; color: #999; font-size: 12px;'>Este es un mensaje automático, por favor no respondas a este correo.</p>
                    </div>",
                IsHtml = true
            };

            return await _emailService.SendEmailAsync(emailRequest);
        }
    }
}
