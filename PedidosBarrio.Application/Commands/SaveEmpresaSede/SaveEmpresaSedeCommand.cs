using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.SaveEmpresaSede
{
    public class SaveEmpresaSedeCommand : IRequest<bool>
    {
        public Guid EmpresaID { get; set; }
        public SaveEmpresaSedeDto Data { get; set; }

        public SaveEmpresaSedeCommand(Guid empresaID, SaveEmpresaSedeDto data)
        {
            EmpresaID = empresaID;
            Data = data;
        }
    }
}
