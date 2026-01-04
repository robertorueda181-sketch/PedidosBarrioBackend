namespace PedidosBarrio.Application.DTOs
{
    /// <summary>
    /// Modelo de horarios para registro de negocio
    /// </summary>
    public class ScheduleDto
    {
        public string? Day { get; set; }
        public string? OpenTime { get; set; }
        public string? CloseTime { get; set; }
        public bool IsClosed { get; set; }
    }

    /// <summary>
    /// DTO para registro de empresa con todos los detalles
    /// Equivalente a RegisterRequest de Angular
    /// </summary>
    public class RegisterDto
    {
        // Datos del propietario
        public string Fullname { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;

        // Datos del negocio
        public string BusinessName { get; set; } = string.Empty;
        public string RUC { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Horarios
        public List<ScheduleDto> Schedules { get; set; } = new();

        // Ubicación
        public string Address { get; set; } = string.Empty;
        public bool UseMap { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Reference { get; set; }

        // Contacto
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Credenciales
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
