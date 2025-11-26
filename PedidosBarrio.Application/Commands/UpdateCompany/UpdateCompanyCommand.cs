using MediatR;
using PedidosBarrio.Application.DTOs;


namespace PedidosBarrio.Application.Commands.UpdateCompany
{
    public class UpdateCompanyCommand : IRequest<Unit> 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Ruc { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressStreet { get; set; }
        public string AddressCity { get; set; }
        public string AddressZipCode { get; set; }

        public UpdateCompanyCommand(Guid id, CreateCompanyDto dto) 
        {
            Id = id;
            Name = dto.Name;
            Ruc = dto.Ruc;
            PhoneNumber = dto.PhoneNumber;
            AddressStreet = dto.AddressStreet;
            AddressCity = dto.AddressCity;
            AddressZipCode = dto.AddressZipCode;
        }
    }
}
