using MediatR;
using PedidosBarrio.Application.DTOs;
using System.Xml.Linq;

namespace PedidosBarrio.Application.Commands.CreateEmpresa
{
    public class CreateCompanyCommand : IRequest<CompanyDto>
    {
        public string Name { get; set; }
        public string Ruc { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressStreet { get; set; }
        public string AddressCity { get; set; }
        public string AddressZipCode { get; set; }

        // Un constructor es útil si creas el comando a partir de un DTO directamente
        public CreateCompanyCommand(CreateCompanyDto dto)
        {
            Name = dto.Name;
            Ruc = dto.Ruc;
            PhoneNumber = dto.PhoneNumber;
            AddressStreet = dto.AddressStreet;
            AddressCity = dto.AddressCity;
            AddressZipCode = dto.AddressZipCode;
        }
    }
}
