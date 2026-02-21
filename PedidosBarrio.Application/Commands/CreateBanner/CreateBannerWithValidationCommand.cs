using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateBanner
{
    public class CreateBannerWithValidationCommand : IRequest<BannerResponseDto>
    {
        public Guid EmpresaID { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public string? TextoBoton { get; set; }
        public string? Link { get; set; }
        public string? Redireccion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public Stream? ImagenStream { get; set; }
        public string? ImagenFileName { get; set; }
        public string? ImagenUrl { get; set; }  // URL directa sin procesar

        public CreateBannerWithValidationCommand(
            Guid empresaID,
            string? titulo,
            string? descripcion,
            string? textoBoton,
            string? link,
            string? redireccion,
            DateTime fechaInicio,
            DateTime fechaFin,
            Stream? imagenStream,
            string? imagenFileName,
            string? imagenUrl = null)
        {
            EmpresaID = empresaID;
            Titulo = titulo;
            Descripcion = descripcion;
            TextoBoton = textoBoton;
            Link = link;
            Redireccion = redireccion;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            ImagenStream = imagenStream;
            ImagenFileName = imagenFileName;
            ImagenUrl = imagenUrl;
        }
    }
}
