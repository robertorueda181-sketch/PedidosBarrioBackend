using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateEmpresa
{
    public class CreateEmpresaCommand : IRequest<EmpresaDto>
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        public string Email { get; set; }
        public string ContrasenaHash { get; set; } // Contraseña en texto plano, se hashea en el handler
        public string Telefono { get; set; }

        public CreateEmpresaCommand(string nombre, string descripcion, string direccion, string referencia, string email, string contrasenaHash, string telefono)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Direccion = direccion;
            Referencia = referencia;
            Email = email;
            ContrasenaHash = contrasenaHash;
            Telefono = telefono;
        }
    }
}
