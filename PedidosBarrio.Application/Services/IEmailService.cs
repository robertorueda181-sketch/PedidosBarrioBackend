using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailRequestDto emailRequest);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName, string businessName, string businessType);
        Task<bool> SendBusinessEvaluationEmailAsync(string toEmail, string userName, string businessName, string businessType);
        Task<string> GetTemplateAsync(string templateName);
    }
}