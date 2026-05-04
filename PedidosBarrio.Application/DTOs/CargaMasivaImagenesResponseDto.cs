using System.Collections.Generic;

namespace PedidosBarrio.Application.DTOs
{
    public class CargaMasivaImagenesResponseDto
    {
        public string Mensaje { get; set; } = string.Empty;
        public List<ImagenCargadaDto> Exitosas { get; set; } = new List<ImagenCargadaDto>();
        public List<string> Errores { get; set; } = new List<string>();
        public bool Exitoso => Errores.Count == 0;
    }
}
