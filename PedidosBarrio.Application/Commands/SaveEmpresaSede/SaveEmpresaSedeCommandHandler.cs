using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using System;
using System.Linq;

namespace PedidosBarrio.Application.Commands.SaveEmpresaSede
{
    public class SaveEmpresaSedeCommandHandler : IRequestHandler<SaveEmpresaSedeCommand, bool>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IDireccionRepository _direccionRepository;
        private readonly INegocioRepository _negocioRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IPasoInicialRepository _pasoInicialRepository;
        private readonly IApplicationLogger _logger;

        public SaveEmpresaSedeCommandHandler(
            IEmpresaRepository empresaRepository,
            IDireccionRepository direccionRepository,
            INegocioRepository negocioRepository,
            IImagenRepository imagenRepository,
            IPasoInicialRepository pasoInicialRepository,
            IApplicationLogger logger)
        {
            _empresaRepository = empresaRepository;
            _direccionRepository = direccionRepository;
            _negocioRepository = negocioRepository;
            _imagenRepository = imagenRepository;
            _pasoInicialRepository = pasoInicialRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(SaveEmpresaSedeCommand request, CancellationToken cancellationToken)
        {
            var empresa = await _empresaRepository.GetByIdAsync(request.EmpresaID);
            if (empresa == null) throw new ApplicationException("Empresa no encontrada");

            // 1. Actualizar Redes Sociales y Telfonos en Empresa
            if (request.Data.RedesSociales != null)
            {
                empresa.Facebook = request.Data.RedesSociales.Facebook;
                empresa.Instagram = request.Data.RedesSociales.Instagram;
                empresa.Twitter = request.Data.RedesSociales.Twitter;
                empresa.Tiktok = request.Data.RedesSociales.Tiktok;
                empresa.Whatsapp = request.Data.RedesSociales.Whatsapp;
            }

            empresa.TelefonoPrincipal = request.Data.Telefono;
            empresa.TelefonoSec = request.Data.Telefono2;
            
            // Actualizar correo en Usuario si se proporciona y ha cambiado
            if (!string.IsNullOrEmpty(request.Data.Correo) && empresa.Usuario != null && empresa.Usuario.Email != request.Data.Correo)
            {
                empresa.Usuario.Email = request.Data.Correo;
            }
            
            await _empresaRepository.UpdateAsync(empresa);
            
            // 2. Actualizar Nombre y Descripción en Negocio
            var negocio = (await _negocioRepository.GetByEmpresaIdAsync(request.EmpresaID)).FirstOrDefault();
            if (negocio != null)
            {
                negocio.Nombre = request.Data.Nombre;
                negocio.Descripcion = request.Data.Descripcion;
                // Si el JSON trae telfono, tambin lo actualizamos en negocio para consistencia (opcional)
                negocio.Telefono = request.Data.Telefono;
                await _negocioRepository.UpdateAsync(negocio);
            }

            // 3. Upsert Dirección (Sede)
            if (request.Data.Direccion != null)
            {
                var direccion = (await _direccionRepository.GetByEmpresaIdAsync(request.EmpresaID)).FirstOrDefault();
                if (direccion == null)
                {
                    direccion = new Direccion(
                        request.EmpresaID,
                        request.Data.Direccion.NombreLocal ?? "Principal", // Default si no viene
                        request.Data.Direccion.DireccionCompleta,
                        request.Data.Direccion.Longitud,
                        request.Data.Direccion.Latitud)
                    {
                        Departamento = request.Data.Direccion.Departamento,
                        Provincia = request.Data.Direccion.Provincia,
                        Distrito = request.Data.Direccion.Distrito,
                        Referencia = request.Data.Direccion.Referencia
                    };
                    await _direccionRepository.AddAsync(direccion);
                }
                else
                {
                    direccion.NombreLocal = request.Data.Direccion.NombreLocal ?? direccion.NombreLocal;
                    direccion.DireccionTexto = request.Data.Direccion.DireccionCompleta;
                    direccion.Longitud = request.Data.Direccion.Longitud;
                    direccion.Latitud = request.Data.Direccion.Latitud;
                    direccion.Departamento = request.Data.Direccion.Departamento;
                    direccion.Provincia = request.Data.Direccion.Provincia;
                    direccion.Distrito = request.Data.Direccion.Distrito;
                    direccion.Referencia = request.Data.Direccion.Referencia ?? direccion.Referencia;
                    await _direccionRepository.UpdateAsync(direccion);
                }
            }

                // 4. Actualizar Logo si se proporciona (como URL o Base64)
                if (!string.IsNullOrEmpty(request.Data.UrlLogo))
                {
                    var imagenes = await _imagenRepository.GetByEmpresaIdAsync(request.EmpresaID);
                    var logo = imagenes.FirstOrDefault(i => i.Type == "LOGO");

                    if (logo == null)
                    {
                        logo = new Imagen
                        {
                            EmpresaID = request.EmpresaID,
                            Type = "LOGO",
                            Urlimagen = request.Data.UrlLogo,
                            FechaRegistro = DateTime.UtcNow,
                            Activa = true,
                            Order = 0
                        };
                        await _imagenRepository.AddAsync(logo);
                    }
                    else
                    {
                        logo.Urlimagen = request.Data.UrlLogo;
                        await _imagenRepository.UpdateAsync(logo);
                    }
                }

                // 5. Auto-completar paso "COMPLETAR_PERFIL" si tiene descripción
                if (!string.IsNullOrWhiteSpace(request.Data.Descripcion))
                {
                    await CompletarPasoAsync(request.EmpresaID, "COMPLETAR_PERFIL", 
                        $"Perfil completado para empresa {request.EmpresaID}");
                }

                // 6. Auto-completar paso "COMPLETAR_DIRECCION" si tiene dirección completa
                if (request.Data.Direccion != null && TieneDireccionCompleta(request.Data.Direccion))
                {
                    await CompletarPasoAsync(request.EmpresaID, "COMPLETAR_DIRECCION", 
                        $"Dirección completada para empresa {request.EmpresaID}");
                }

                return true;
            }

            private bool TieneDireccionCompleta(SaveDireccionSedeDto direccion)
            {
                // Valida que la dirección tenga datos suficientes
                return !string.IsNullOrWhiteSpace(direccion.DireccionCompleta) &&
                       (direccion.Latitud != 0 || direccion.Longitud != 0);
            }

            private async Task CompletarPasoAsync(Guid empresaId, string codigoPaso, string logMessage)
            {
                try
                {
                    var paso = await _pasoInicialRepository.GetPasoPorCodigoAsync(empresaId, codigoPaso);
                    if (paso != null && !paso.Completado)
                    {
                        await _pasoInicialRepository.CompletarPasoAsync(paso.PasoID);
                        await _logger.LogInformationAsync(logMessage);
                    }
                }
                catch (Exception ex)
                {
                    // No fallar el guardado si hay error al completar el paso
                    await _logger.LogWarningAsync($"Error al marcar paso {codigoPaso} como completado: {ex.Message}");
                }
            }
    }
}
