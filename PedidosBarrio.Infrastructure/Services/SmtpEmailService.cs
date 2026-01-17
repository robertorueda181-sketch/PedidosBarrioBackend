using Microsoft.Extensions.Configuration;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PedidosBarrio.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationLogger _logger;

        public SmtpEmailService(IConfiguration configuration, IApplicationLogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailRequestDto emailRequest)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("Email:Smtp");
                
                var smtpClient = new SmtpClient(smtpSettings["Host"])
                {
                    Port = int.Parse(smtpSettings["Port"]),
                    Credentials = new NetworkCredential(
                        smtpSettings["Username"], 
                        smtpSettings["Password"]
                    ),
                    EnableSsl = bool.Parse(smtpSettings["EnableSsl"])
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(
                        smtpSettings["FromEmail"], 
                        smtpSettings["FromName"]
                    ),
                    Subject = emailRequest.Subject,
                    Body = emailRequest.Body,
                    IsBodyHtml = emailRequest.IsHtml
                };

                mailMessage.To.Add(new MailAddress(emailRequest.To, emailRequest.ToName));

                // Agregar attachments si existen
                if (emailRequest.Attachments?.Any() == true)
                {
                    foreach (var attachment in emailRequest.Attachments)
                    {
                        var stream = new MemoryStream(attachment.Content);
                        mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
                    }
                }

                await smtpClient.SendMailAsync(mailMessage);

                await _logger.LogInformationAsync(
                    $"Email enviado exitosamente a: {emailRequest.To}",
                    "EmailService");

                return true;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    $"Error al enviar email a {emailRequest.To}: {ex.Message}",
                    ex,
                    "EmailService");
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName, string businessName, string businessType)
        {
            try
            {
                var emailBody = GenerateWelcomeEmailTemplate(userName, businessName, businessType);

                var emailRequest = new EmailRequestDto
                {
                    To = toEmail,
                    ToName = userName,
                    Subject = "¡Bienvenido a PedidosBarrio! - Tu registro está siendo evaluado",
                    Body = emailBody,
                    IsHtml = true
                };

                return await SendEmailAsync(emailRequest);

            }
            catch(Exception ex)
            {

                Console.Write(ex.Message);
                return false;
            }
           
        }

        public async Task<bool> SendBusinessEvaluationEmailAsync(string toEmail, string userName, string businessName, string businessType)
        {
            var emailBody = GenerateEvaluationEmailTemplate(userName, businessName, businessType);
            
            var emailRequest = new EmailRequestDto
            {
                To = toEmail,
                ToName = userName,
                Subject = $"Tu {businessType.ToLower()} está siendo evaluado - PedidosBarrio",
                Body = emailBody,
                IsHtml = true
            };

            return await SendEmailAsync(emailRequest);
        }

        private string GenerateWelcomeEmailTemplate(string userName, string businessName, string businessType)
        {
            var tipoNegocioTexto = businessType.ToLower() switch
            {
                "negocio" => "negocio",
                "servicio" => "servicio", 
                "inmueble" => "inmueble",
                _ => "publicación"
            };

            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang='es'>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='UTF-8'>");
            html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            html.AppendLine("    <title>Bienvenido a PedidosBarrio</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }");
            html.AppendLine("        .header { background-color: #2196F3; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }");
            html.AppendLine("        .content { background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }");
            html.AppendLine("        .footer { background-color: #333; color: white; padding: 15px; text-align: center; border-radius: 0 0 8px 8px; font-size: 12px; }");
            html.AppendLine("        .highlight { background-color: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; }");
            html.AppendLine("        .button { background-color: #28a745; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; display: inline-block; margin: 20px 0; }");
            html.AppendLine("        .warning { background-color: #f8d7da; padding: 15px; border-left: 4px solid #dc3545; margin: 20px 0; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            html.AppendLine("    <div class='header'>");
            html.AppendLine("        <h1>🎉 ¡Bienvenido a PedidosBarrio!</h1>");
            html.AppendLine("        <p>Tu registro ha sido recibido exitosamente</p>");
            html.AppendLine("    </div>");

            html.AppendLine("    <div class='content'>");
            html.AppendLine($"        <h2>Hola {userName},</h2>");
            html.AppendLine($"        <p>¡Gracias por registrar <strong>'{businessName}'</strong> en PedidosBarrio!</p>");
            
            html.AppendLine("        <div class='highlight'>");
            html.AppendLine("            <h3>📋 Proceso de Evaluación</h3>");
            html.AppendLine($"            <p>Tu {tipoNegocioTexto} está actualmente siendo evaluado por nuestro equipo siguiendo nuestras políticas de calidad y seguridad.</p>");
            html.AppendLine("        </div>");

            html.AppendLine("        <h3>⏰ ¿Cuánto tiempo tomará?</h3>");
            html.AppendLine("        <ul>");
            html.AppendLine("            <li><strong>Tiempo máximo:</strong> 24 horas</li>");
            html.AppendLine("            <li><strong>Horario de evaluación:</strong> Lunes a Viernes de 9:00 AM a 6:00 PM</li>");
            html.AppendLine("            <li><strong>Notificación:</strong> Te avisaremos por email cuando esté aprobado</li>");
            html.AppendLine("        </ul>");

            html.AppendLine("        <h3>✅ ¿Qué sigue después?</h3>");
            html.AppendLine("        <ol>");
            html.AppendLine("            <li>Revisaremos la información de tu registro</li>");
            html.AppendLine("            <li>Verificaremos que cumple con nuestras políticas</li>");
            html.AppendLine($"            <li>Una vez aprobado, tu {tipoNegocioTexto} aparecerá en nuestra página principal</li>");
            html.AppendLine("            <li>¡Los clientes podrán encontrarte y contactarte!</li>");
            html.AppendLine("        </ol>");

            html.AppendLine("        <div class='warning'>");
            html.AppendLine("            <h3>⚠️ Importante</h3>");
            html.AppendLine($"            <p>Mientras tu {tipoNegocioTexto} está siendo evaluado, <strong>no aparecerá</strong> en los resultados de búsqueda ni en la página principal. Una vez aprobado, estará completamente visible para todos los usuarios.</p>");
            html.AppendLine("        </div>");

            html.AppendLine("        <h3>📞 ¿Necesitas ayuda?</h3>");
            html.AppendLine("        <p>Si tienes alguna pregunta, no dudes en contactarnos:</p>");
            html.AppendLine("        <ul>");
            html.AppendLine("            <li>📧 Email: soporte@pedidosbarrio.com</li>");
            html.AppendLine("            <li>📱 WhatsApp: +57 300 123 4567</li>");
            html.AppendLine("        </ul>");

            html.AppendLine("        <p>¡Gracias por formar parte de la comunidad PedidosBarrio!</p>");
            html.AppendLine("        <p><strong>El equipo de PedidosBarrio</strong></p>");
            html.AppendLine("    </div>");

            html.AppendLine("    <div class='footer'>");
            html.AppendLine("        <p>&copy; 2024 PedidosBarrio. Todos los derechos reservados.</p>");
            html.AppendLine("        <p>Este correo fue enviado automáticamente. Por favor, no respondas a este email.</p>");
            html.AppendLine("    </div>");

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private string GenerateEvaluationEmailTemplate(string userName, string businessName, string businessType)
        {
            // Este template es similar pero con un enfoque específico en la evaluación
            return GenerateWelcomeEmailTemplate(userName, businessName, businessType);
        }
    }
}