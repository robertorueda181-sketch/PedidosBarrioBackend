using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.SaveEmpresaSede
{
    public class SaveEmpresaSedeCommandHandler : IRequestHandler<SaveEmpresaSedeCommand, bool>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IDireccionRepository _direccionRepository;
        private readonly INegocioRepository _negocioRepository;
        private readonly IImagenRepository _imagenRepository;
        private readonly IPasoInicialRepository _pasoInicialRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;

        public SaveEmpresaSedeCommandHandler(
            IEmpresaRepository empresaRepository,
            IDireccionRepository direccionRepository,
            INegocioRepository negocioRepository,
            IImagenRepository imagenRepository,
            IPasoInicialRepository pasoInicialRepository,
            ICurrentUserService currentUserService,
            IApplicationLogger logger)
        {
            _empresaRepository = empresaRepository;
            _direccionRepository = direccionRepository;
            _negocioRepository = negocioRepository;
            _imagenRepository = imagenRepository;
            _pasoInicialRepository = pasoInicialRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(SaveEmpresaSedeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var empresa = await _empresaRepository.GetByIdAsync(request.EmpresaID);
                if (empresa == null) 
                    throw new ApplicationException("Empresa no encontrada");

                // 1. Actualizar Redes Sociales y Teléfonos en Empresa
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
 

                await _empresaRepository.UpdateAsync(empresa);
                await _logger.LogInformationAsync($"Redes sociales y teléfonos actualizados para empresa {request.EmpresaID}");

                // 2. Actualizar Nombre y Descripción en Negocio
                var negocio = (await _negocioRepository.GetByEmpresaIdAsync(request.EmpresaID)).FirstOrDefault();
                if (negocio != null)
                {
                    negocio.Nombre = request.Data.Nombre;
                    negocio.Descripcion = request.Data.Descripcion;
                    await _negocioRepository.UpdateAsync(negocio);
                    await _logger.LogInformationAsync($"Nombre y descripción actualizados para negocio {negocio.NegocioID}");
                }

                // 3. Upsert Dirección (Sede) - Obtiene de la tabla DIRECCION única por empresaID
                if (request.Data.Direccion != null)
                {
                    var direccion = (await _direccionRepository.GetByEmpresaIdAsync(request.EmpresaID)).FirstOrDefault();

                    if (direccion == null)
                    {
                        // Crear nueva dirección
                        direccion = new Direccion(
                            request.EmpresaID,
                            request.Data.Direccion.NombreLocal ?? "Principal",
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
                        await _logger.LogInformationAsync($"Nueva dirección creada para empresa {request.EmpresaID}");
                    }
                    else
                    {
                        // Actualizar dirección existente
                        direccion.NombreLocal = request.Data.Direccion.NombreLocal ?? direccion.NombreLocal;
                        direccion.DireccionTexto = request.Data.Direccion.DireccionCompleta;
                        direccion.Longitud = request.Data.Direccion.Longitud;
                        direccion.Latitud = request.Data.Direccion.Latitud;
                        direccion.Departamento = request.Data.Direccion.Departamento ?? direccion.Departamento;
                        direccion.Provincia = request.Data.Direccion.Provincia ?? direccion.Provincia;
                        direccion.Distrito = request.Data.Direccion.Distrito ?? direccion.Distrito;
                        direccion.Referencia = request.Data.Direccion.Referencia ?? direccion.Referencia;
                        await _direccionRepository.UpdateAsync(direccion);
                        await _logger.LogInformationAsync($"Dirección actualizada para empresa {request.EmpresaID}");
                    }
                }

                // 4. Actualizar Logo si se proporciona
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
                        await _logger.LogInformationAsync($"Logo creado para empresa {request.EmpresaID}");
                    }
                    else
                    {
                        logo.Urlimagen = request.Data.UrlLogo;
                        await _imagenRepository.UpdateAsync(logo);
                        await _logger.LogInformationAsync($"Logo actualizado para empresa {request.EmpresaID}");
                    }
                }

                // 5. Auto-completar paso "COMPLETAR_PERFIL" si tiene descripción (solo si PasosIniciales es true en token)
                if (_currentUserService.GetPasosIniciales() && !string.IsNullOrWhiteSpace(request.Data.Descripcion))
                {
                    await CompletarPasoAsync(request.EmpresaID, "COMPLETAR_PERFIL", 
                        $"Perfil completado para empresa {request.EmpresaID}");
                }

                // 6. Auto-completar paso "COMPLETAR_DIRECCION" si tiene dirección completa (solo si PasosIniciales es true en token)
                if (_currentUserService.GetPasosIniciales() && request.Data.Direccion != null && TieneDireccionCompleta(request.Data.Direccion))
                {
                    await CompletarPasoAsync(request.EmpresaID, "COMPLETAR_DIRECCION", 
                        $"Dirección completada para empresa {request.EmpresaID}");
                }

                return true;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error al guardar sede de empresa {request.EmpresaID}: {ex.Message}", ex);
                throw;
            }
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

                            // Verificar si TODOS los pasos iniciales están completos
                            await VerificarYFinalizarPasosInicialesAsync(empresaId);
                        }
                    }
                    catch (Exception ex)
                    {
                        // No fallar el guardado si hay error al completar el paso
                        await _logger.LogWarningAsync($"Error al marcar paso {codigoPaso} como completado: {ex.Message}");
                    }
                }

                /// <summary>
                /// Verifica si todos los pasos iniciales están completados
                /// Si es así, marca la empresa como visible y desactiva PasosIniciales
                /// </summary>
                private async Task VerificarYFinalizarPasosInicialesAsync(Guid empresaId)
                {
                    try
                    {
                        // Obtener todos los pasos iniciales de la empresa
                        var todosLosPasos = await _pasoInicialRepository.GetPasosPorEmpresaAsync(empresaId);

                        if (todosLosPasos == null || !todosLosPasos.Any())
                        {
                            return;
                        }

                        // Verificar si TODOS los pasos están completados
                        var todosCompletados = todosLosPasos.All(p => p.Completado);

                        if (todosCompletados)
                        {
                            // Obtener la empresa
                            var empresa = await _empresaRepository.GetByIdAsync(empresaId);
                            if (empresa != null)
                            {
                                // Marcar como visible y desactivar evaluación de pasos iniciales
                                empresa.Visible = true;
                                empresa.PasosIniciales = false;

                                await _empresaRepository.UpdateAsync(empresa);
                                await _logger.LogInformationAsync(
                                    $"Empresa {empresaId} finalizada: Visible=true, PasosIniciales=false. Todos los pasos iniciales completados.",
                                    "SaveEmpresaSedeCommand");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // No fallar si hay error al finalizar pasos
                        await _logger.LogWarningAsync(
                            $"Error al verificar finalización de pasos para empresa {empresaId}: {ex.Message}");
                    }
                }
            }
        }

