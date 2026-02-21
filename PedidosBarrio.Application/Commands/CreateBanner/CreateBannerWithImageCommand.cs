using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.CreateBanner
{
        public class CreateBannerWithImageCommand : IRequest<BannerResponseDto>
        {
            public Guid EmpresaID { get; set; }
            public string? Titulo { get; set; }
            public string? Descripcion { get; set; }
            public string? TextoBoton { get; set; }
            public string? Link { get; set; }
            public string? Redireccion { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public DateTime FechaExpiracion { get; set; }
            public bool? Visible { get; set; }
            public bool? Aprobado { get; set; }
            public short Prioridad { get; set; }
            public Stream? ImagenStream { get; set; }
            public string? ImagenFileName { get; set; }

            public CreateBannerWithImageCommand(
                Guid empresaID,
                string? titulo,
                string? descripcion,
                string? textoBoton,
                string? link,
                string? redireccion,
                DateTime fechaInicio,
                DateTime fechaFin,
                bool? visible = true,
                bool? aprobado = false,
                short prioridad = 1,
                Stream? imagenStream = null,
                string? imagenFileName = null)
            {
                EmpresaID = empresaID;
                Titulo = titulo;
                Descripcion = descripcion;
                TextoBoton = textoBoton;
                Link = link;
                Redireccion = redireccion;
                FechaInicio = fechaInicio;
                FechaFin = fechaFin;
                Visible = visible;
                Aprobado = aprobado;
                Prioridad = prioridad;
                ImagenStream = imagenStream;
                ImagenFileName = imagenFileName;
            }
        }
    }
