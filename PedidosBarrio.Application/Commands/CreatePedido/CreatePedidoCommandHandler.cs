using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Commands.CreatePedido;

public class CreatePedidoCommandHandler : IRequestHandler<CreatePedidoCommand, CreatePedidoResponse>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IPedidoDetalleRepository _pedidoDetalleRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly INegocioRepository _negocioRepository;
    private readonly IApplicationLogger _logger;

    public CreatePedidoCommandHandler(
        IClienteRepository clienteRepository,
        IPedidoRepository pedidoRepository,
        IPedidoDetalleRepository pedidoDetalleRepository,
        IProductoRepository productoRepository,
        INegocioRepository negocioRepository,
        IApplicationLogger logger)
    {
        _clienteRepository = clienteRepository;
        _pedidoRepository = pedidoRepository;
        _pedidoDetalleRepository = pedidoDetalleRepository;
        _productoRepository = productoRepository;
        _negocioRepository = negocioRepository;
        _logger = logger;
    }

    public async Task<CreatePedidoResponse> Handle(CreatePedidoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var codigo = request.Data.Codigo?.Trim();
            var clienteDto = request.Data.Cliente;

            // Validar código del negocio
            if (string.IsNullOrEmpty(codigo))
            {
                throw new InvalidOperationException("El código del negocio es requerido");
            }

            // Obtener empresa por código del negocio
            var empresa = await _negocioRepository.GetByCodigoEmpresaAsync(codigo);
            if (empresa == null)
            {
                throw new InvalidOperationException($"Negocio con código {codigo} no encontrado");
            }

            var empresaId = empresa.ID;

            // 1. Obtener o crear cliente
            var cliente = await _clienteRepository.GetByDniAsync(clienteDto.DNI);

            if (cliente == null)
            {
                // Usar un usuario por defecto para clientes públicos
                var usuarioIdDefault = Guid.Parse("00000000-0000-0000-0000-000000000000");
                
                cliente = new Cliente(usuarioIdDefault, clienteDto.DNI, clienteDto.Nombres)
                {
                    Telefono = clienteDto.Telefono,
                    DireccionTexto = clienteDto.DireccionTexto,
                    Latitud = clienteDto.Latitud,
                    Longitud = clienteDto.Longitud,
                    Distrito = clienteDto.Distrito,
                    Provincia = clienteDto.Provincia,
                    Departamento = clienteDto.Departamento
                };

                await _clienteRepository.AddAsync(cliente);
                await _logger.LogInformationAsync($"Cliente creado: {clienteDto.DNI} para negocio {codigo}");
            }
            else
            {
                // Actualizar datos del cliente si ya existe
                cliente.Telefono = clienteDto.Telefono ?? cliente.Telefono;
                cliente.DireccionTexto = clienteDto.DireccionTexto ?? cliente.DireccionTexto;
                cliente.Latitud = clienteDto.Latitud ?? cliente.Latitud;
                cliente.Longitud = clienteDto.Longitud ?? cliente.Longitud;
                cliente.Distrito = clienteDto.Distrito ?? cliente.Distrito;
                cliente.Provincia = clienteDto.Provincia ?? cliente.Provincia;
                cliente.Departamento = clienteDto.Departamento ?? cliente.Departamento;

                await _clienteRepository.UpdateAsync(cliente);
                await _logger.LogInformationAsync($"Cliente actualizado: {clienteDto.DNI}");
            }

            // 2. Crear pedido
            var pedido = new Pedido(empresaId, cliente.ClienteID)
            {
                Observaciones = request.Data.Observaciones
            };

            var pedidoId = await _pedidoRepository.AddAsync(pedido);
            await _logger.LogInformationAsync($"Pedido creado: {pedidoId} para cliente {cliente.DNI}");

            // 3. Crear detalles del pedido
            var detalles = new List<PedidoDetalle>();
            decimal total = 0;

            foreach (var productoDto in request.Data.Productos)
            {
                // Verificar que el producto existe y pertenece a la empresa
                var producto = await _productoRepository.GetByIdAsync(productoDto.ProductoID, empresaId);
                if (producto == null)
                {
                    throw new InvalidOperationException($"Producto {productoDto.ProductoID} no encontrado para esta empresa");
                }

                var detalle = new PedidoDetalle(
                    pedidoId,
                    productoDto.ProductoID,
                    productoDto.Cantidad,
                    productoDto.PrecioUnitario);

                detalles.Add(detalle);
                total += detalle.Subtotal;

                // Reducir stock si el producto tiene inventario
                if (producto.Inventario)
                {
                    producto.Stock -= productoDto.Cantidad;
                    await _productoRepository.UpdateAsync(producto);
                    await _logger.LogInformationAsync($"Stock reducido para producto {productoDto.ProductoID}: -{productoDto.Cantidad}");
                }
            }

            // Guardar detalles del pedido
            if (detalles.Count > 0)
            {
                await _pedidoDetalleRepository.AddBulkAsync(detalles);
                await _logger.LogInformationAsync($"Se agregaron {detalles.Count} detalles al pedido {pedidoId}");
            }

            // 4. Actualizar total del pedido
            pedido.Total = total;
            await _pedidoRepository.UpdateAsync(pedido);

            await _logger.LogInformationAsync($"Pedido {pedidoId} completado con total: {total}");

            return new CreatePedidoResponse(pedidoId, pedido.PedidoUID, total);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Error al crear pedido: {ex.Message}", ex);
            throw;
        }
    }
}
