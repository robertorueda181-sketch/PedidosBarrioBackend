using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.SocialLogin
{
    /// <summary>
    /// Command para login social (Google, Facebook, etc.)
    /// Si el usuario no existe, lo registra automáticamente
    /// </summary>
    public class SocialLoginCommand : IRequest<SocialLoginResponseDto>
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Provider { get; set; }
        public string IdToken { get; set; }

        public SocialLoginCommand(SocialLoginRequest request)
        {
            Id = request.Id;
            Email = request.Email;
            FirstName = request.FirstName;
            LastName = request.LastName;
            Provider = request.Provider;
            IdToken = request.IdToken;
        }

        public SocialLoginCommand(
            string id,
            string email,
            string firstName,
            string lastName,
            string provider,
            string idToken)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Provider = provider;
            IdToken = idToken;
        }
    }
}
