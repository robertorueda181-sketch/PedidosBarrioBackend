using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Repositories;
using System.Linq;

namespace PedidosBarrio.Application.Queries.GetEmpresaSedeDetalle
{
    public class GetEmpresaSedeDetalleQueryHandler : IRequestHandler<GetEmpresaSedeDetalleQuery, EmpresaSedeDetalleDto>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IDireccionRepository _direccionRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly INegocioRepository _negocioRepository;
        private readonly IPiiEncryptionService _encryptionService;

        public GetEmpresaSedeDetalleQueryHandler(
            IEmpresaRepository empresaRepository,
            IDireccionRepository direccionRepository,
            IImagenRepository imagenRepository,
            IImageProcessingService imageProcessingService,
            INegocioRepository negocioRepository,
            IPiiEncryptionService encryptionService)
        {
            _empresaRepository = empresaRepository;
            _direccionRepository = direccionRepository;
            _imagenRepository = imagenRepository;
            _imageProcessingService = imageProcessingService;
            _negocioRepository = negocioRepository;
            _encryptionService = encryptionService;
        }

        public async Task<EmpresaSedeDetalleDto> Handle(GetEmpresaSedeDetalleQuery request, CancellationToken cancellationToken)
        {
            var empresa = await _empresaRepository.GetByIdAsync(request.EmpresaID);
            if (empresa == null) return null;

            // Obtener datos del negocio para nombre y descripcin (si no estn en empresa)
            var negocio = (await _negocioRepository.GetByEmpresaIdAsync(request.EmpresaID)).FirstOrDefault();

            // Obtener direccin (Sede)
            var direccion = (await _direccionRepository.GetByEmpresaIdAsync(request.EmpresaID)).FirstOrDefault();

            // Obtener Logo
            var imagenes = await _imagenRepository.GetByEmpresaIdAsync(request.EmpresaID);
            
            string? logoUrl = null;
          

                    // Obtener Imagen de Perfil
                    var profileImage = imagenes.FirstOrDefault(i => i.Type == "PROFILE");
                    string? profileImageUrl = null;
                    if (profileImage != null && !string.IsNullOrEmpty(profileImage.Urlimagen))
                    {
                        profileImageUrl = await _imageProcessingService.GetImageUrlAsync(profileImage.Urlimagen);
                    }

                    // Desencriptar email
                    var emailDesencriptado = empresa.Usuario?.Email != null 
                        ? _encryptionService.Decrypt(empresa.Usuario.Email) 
                        : null;

                    return new EmpresaSedeDetalleDto
                    {
                        Codigo = negocio.Codigo,
                        Nombre = negocio?.Nombre ?? "",
                        Descripcion = negocio?.Descripcion ?? string.Empty,
                        Email = emailDesencriptado,
                        LogoUrl = logoUrl,
                        ProfileImageUrl = profileImageUrl,
                        Facebook = empresa.Facebook,
                        Instagram = empresa.Instagram,
                        Twitter = empresa.Twitter,
                        Tiktok = empresa.Tiktok,
                        Whatsapp = empresa.Whatsapp,
                        TelefonoPrincipal = empresa.TelefonoPrincipal,
                        TelefonoSec = empresa.TelefonoSec,
                        DireccionID = direccion?.DireccionID,
                        NombreLocal = direccion?.NombreLocal,
                        Direccion = direccion?.DireccionTexto,
                        Longitud = direccion?.Longitud ?? 0,
                        Latitud = direccion?.Latitud ?? 0,
                        Departamento = direccion?.Departamento,
                        Provincia = direccion?.Provincia,
                        Distrito = direccion?.Distrito,
                        Referencia = direccion?.Referencia
                    };
                }
            }
}
