using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreatePresentacion
{
    public class CreatePresentacionCommandHandler : IRequestHandler<CreatePresentacionCommand, PresentacionDetalleDto>
    {
        private readonly IPresentacionRepository _presentacionRepository;
        private readonly IPresentacionOpcionRepository _presentacionOpcionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationLogger _logger;

        public CreatePresentacionCommandHandler(
            IPresentacionRepository presentacionRepository,
            IPresentacionOpcionRepository presentacionOpcionRepository,
            ICurrentUserService currentUserService,
            IApplicationLogger logger)
        {
            _presentacionRepository = presentacionRepository;
            _presentacionOpcionRepository = presentacionOpcionRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<PresentacionDetalleDto> Handle(CreatePresentacionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var empresaId = _currentUserService.GetEmpresaId();

                await _logger.LogInformationAsync(
                    $"Creando presentación '{request.Descripcion}' para producto {request.ProductoID}",
                    "CreatePresentacionCommand");

                // Crear la presentación
                var presentacion = new Presentacion(request.Descripcion, empresaId, request.ProductoID);
                await _presentacionRepository.AddAsync(presentacion);

                // Crear las opciones
                var opciones = new List<PresentacionOpcion>();
                foreach (var opcionDto in request.Opciones)
                {
                    var opcion = new PresentacionOpcion(
                        opcionDto.Valor,
                        presentacion.PresentacionID,
                        opcionDto.Precio,
                        opcionDto.Imagen
                    )
                    {
                        Descripcion = opcionDto.Descripcion,
                        Stock = opcionDto.Stock,
                        Activa = true
                    };

                    await _presentacionOpcionRepository.AddAsync(opcion);
                    opciones.Add(opcion);
                }

                // Crear DTO de respuesta
                var result = new PresentacionDetalleDto
                {
                    PresentacionID = presentacion.PresentacionID,
                    Descripcion = presentacion.Descripcion,
                    ProductoID = presentacion.ProductoID,
                    Activa = presentacion.Activa,
                    Opciones = opciones.Select(o => new PresentacionOpcionDto
                    {
                        PresentacionOpcionID = o.PresentacionOpcionID,
                        Valor = o.Valor,
                        PresentacionID = o.PresentacionID,
                        Precio = o.Precio,
                        Imagen = o.Imagen,
                        Descripcion = o.Descripcion,
                        Activa = o.Activa,
                        Stock = o.Stock
                    }).ToList()
                };

                await _logger.LogInformationAsync(
                    $"Presentación creada exitosamente con {result.Opciones.Count} opciones",
                    "CreatePresentacionCommand");

                return result;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(
                    $"Error al crear presentación: {ex.Message}",
                    ex,
                    "CreatePresentacionCommand");
                throw new ApplicationException($"Error al crear la presentación: {ex.Message}", ex);
            }
        }
    }
}
