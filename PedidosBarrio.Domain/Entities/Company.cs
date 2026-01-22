using System;
using System.Collections.Generic;
using System.Text;

namespace PedidosBarrio.Domain.Entities
{
    public class Company
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Ruc { get; private set; }
        public string PhoneNumber { get; private set; } 
        public string AddressStreet { get; private set; }
        public string AddressCity { get; private set; }
        public string AddressZipCode { get; private set; }

        private Company() { }

        public Company(string name, string ruc, string phoneNumber, string addressStreet, string addressCity, string addressZipCode)
        {
            Id = Guid.NewGuid();
            Name = name;
            Ruc = ruc;
            PhoneNumber = phoneNumber;
            AddressStreet = addressStreet;
            AddressCity = addressCity;
            AddressZipCode = addressZipCode;
        }

        public void UpdateInformation(string newName, string newPhoneNumber, string newAddressStreet, string newAddressCity, string newAddressZipCode)
        {
            Name = newName;
            PhoneNumber = newPhoneNumber;
            AddressStreet = newAddressStreet;
            AddressCity = newAddressCity;
            AddressZipCode = newAddressZipCode;
        }
    }
}
