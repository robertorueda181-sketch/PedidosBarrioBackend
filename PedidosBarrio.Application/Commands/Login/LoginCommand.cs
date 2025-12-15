using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.Login
{
    public class LoginCommand : IRequest<LoginResponseDto>
    {
        public string Email { get; set; }
        public string Contrasena { get; set; }

        public LoginCommand(string email, string contrasena)
        {
            Email = email;
            Contrasena = contrasena;
        }
    }
}
