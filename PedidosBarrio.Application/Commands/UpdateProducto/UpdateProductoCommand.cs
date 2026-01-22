using MediatR;
using PedidosBarrio.Application.DTOs;

namespace PedidosBarrio.Application.Commands.UpdateProducto
{
    public class UpdateProductoCommand : IRequest<ProductoDto>
    {
        public int ProductoID { get; set; }
        public short CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
                public int? StockMinimo { get; set; }
                public bool Inventario { get; set; }
                public bool Visible { get; set; }
                public List<PrecioDto> Precios { get; set; }

                public UpdateProductoCommand(int productoId, UpdateProductoDto dto)
                {
                    ProductoID = productoId;
                    CategoriaID = dto.CategoriaID;
                    Nombre = dto.Nombre;
                    Descripcion = dto.Descripcion;
                    Stock = dto.Stock;
                    StockMinimo = dto.StockMinimo;
                    Inventario = dto.Inventario;
                    Visible = dto.Visible;
                    Precios = dto.Precios;
                }
            }
        }
