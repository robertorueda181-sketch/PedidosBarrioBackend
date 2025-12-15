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
        public string Contrasena { get; set; } 
        public string Telefono { get; set; }

        public CreateEmpresaCommand(string nombre, string descripcion, string direccion, string referencia, string email, string contrasena, string telefono)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Direccion = direccion;
            Referencia = referencia;
            Email = email;
            Contrasena = contrasena;
            Telefono = telefono;
        }
    }
}
