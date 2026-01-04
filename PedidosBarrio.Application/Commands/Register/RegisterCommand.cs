using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.Register
{
    /// <summary>
    /// Command para registrar una nueva empresa con todos sus detalles
    /// </summary>
    public class RegisterCommand : IRequest<LoginResponseDto>
    {
        public string Fullname { get; set; }
        public string DNI { get; set; }
        public string BusinessName { get; set; }
        public string RUC { get; set; }
        public string Category { get; set; }
        public string? Description { get; set; }
        public List<ScheduleDto> Schedules { get; set; }
        public string Address { get; set; }
        public bool UseMap { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Reference { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public RegisterCommand(RegisterDto dto)
        {
            Fullname = dto.Fullname;
            DNI = dto.DNI;
            BusinessName = dto.BusinessName;
            RUC = dto.RUC;
            Category = dto.Category;
            Description = dto.Description;
            Schedules = dto.Schedules;
            Address = dto.Address;
            UseMap = dto.UseMap;
            Latitude = dto.Latitude;
            Longitude = dto.Longitude;
            Reference = dto.Reference;
            Phone = dto.Phone;
            Email = dto.Email;
            Username = dto.Username;
            Password = dto.Password;
        }

        // Constructor adicional con parámetros individuales
        public RegisterCommand(
            string fullname,
            string dni,
            string businessName,
            string ruc,
            string category,
            string? description,
            List<ScheduleDto> schedules,
            string address,
            bool useMap,
            double? latitude,
            double? longitude,
            string? reference,
            string phone,
            string email,
            string username,
            string password)
        {
            Fullname = fullname;
            DNI = dni;
            BusinessName = businessName;
            RUC = ruc;
            Category = category;
            Description = description;
            Schedules = schedules;
            Address = address;
            UseMap = useMap;
            Latitude = latitude;
            Longitude = longitude;
            Reference = reference;
            Phone = phone;
            Email = email;
            Username = username;
            Password = password;
        }
    }
}
