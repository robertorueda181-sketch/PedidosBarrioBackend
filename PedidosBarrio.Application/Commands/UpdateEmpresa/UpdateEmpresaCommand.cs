using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UpdateEmpresa
{
    public class UpdateEmpresaCommand : IRequest<EmpresaDto>
    {
        public Guid EmpresaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string Referencia { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public bool Activa { get; set; }

        public UpdateEmpresaCommand(Guid empresaID, string nombre, string descripcion, string direccion, string referencia, string email, string telefono, bool activa)
        {
            EmpresaID = empresaID;
            Nombre = nombre;
            Descripcion = descripcion;
            Direccion = direccion;
            Referencia = referencia;
            Email = email;
            Telefono = telefono;
            Activa = activa;
        }
    }
}
