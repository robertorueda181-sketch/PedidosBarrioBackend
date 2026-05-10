using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.GuardarFormularioContacto;

public class GuardarFormularioContactoCommandHandler : IRequestHandler<GuardarFormularioContactoCommand, FormularioContactoResponseDto>
{
    private readonly IFormularioContactoRepository _formularioContactoRepository;
    private readonly IApplicationLogger _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly INegocioRepository _negocioRepository;

    public GuardarFormularioContactoCommandHandler(
        IFormularioContactoRepository formularioContactoRepository,
        ICurrentUserService currentUserService,
        INegocioRepository negocioRepository,
        IApplicationLogger logger)
    {
        _formularioContactoRepository = formularioContactoRepository;
        _logger = logger;
        _currentUserService = currentUserService;
        _negocioRepository = negocioRepository;
    }

    public async Task<FormularioContactoResponseDto> Handle(GuardarFormularioContactoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Data;

            Guid empresaId = Guid.Empty;
            try
            {
                empresaId = _currentUserService.GetEmpresaId();
            }
            catch (Exception)
            {
            }
            if (empresaId == Guid.Empty)
            {
                var empresa = await _negocioRepository.GetByCodigoEmpresaAsync(dto.Codigo!);

                empresaId = empresa.EmpresaID;
            }


            // Crear la entidad del formulario de contacto
            var formulario = new FormularioContacto
            {
                Nombre = dto.Nombre.Trim(),
                Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim(),
                EmpresaID = empresaId,
                FechaRegistro = DateTime.UtcNow,
                FechaReserva = dto.FechaReserva.Date,
                HoraReserva = dto.HoraReserva,
                Comentarios = dto.Comentarios,
                Ocasion = dto.Ocasion,
                Activa = true
            };

            // Guardar en la base de datos y obtener el ID generado
            var formularioId = await _formularioContactoRepository.AddAsync(formulario);

            await _logger.LogInformationAsync($"Formulario de contacto guardado: ID={formularioId}, FechaReserva={formulario.FechaReserva}");

            // Retornar respuesta
            return new FormularioContactoResponseDto
            {
                FormularioContactoID = formularioId,
                Id = formulario.Id,
                Nombre = formulario.Nombre,
                Telefono = formulario.Telefono,
                FechaReserva = formulario.FechaReserva,
                HoraReserva = formulario.HoraReserva,
                NumeroPersonas = formulario.NumeroPersonas,
                Ocasion = formulario.Ocasion,
                Comentarios = formulario.Comentarios,
                EmpresaID = formulario.EmpresaID,
                FechaRegistro = formulario.FechaRegistro
            };
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Error al guardar formulario de contacto: {ex.Message}", ex);
            throw;
        }
    }
}
