using AutoMapper;
using MediatR;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Repositories;

namespace PedidosBarrio.Application.Queries.GetNegocioByCodigoEmpresa
{
    public class GetNegocioByCodigoEmpresaQueryHandler : IRequestHandler<GetNegocioByCodigoEmpresaQuery, NegocioDetalleDto>
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly INegocioRepository _negocioRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;

        public GetNegocioByCodigoEmpresaQueryHandler(
            IEmpresaRepository empresaRepository,
            INegocioRepository negocioRepository,
            IProductoRepository productoRepository,
            ICategoriaRepository categoriaRepository,
            IMapper mapper)
        {
            _empresaRepository = empresaRepository;
            _negocioRepository = negocioRepository;
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }

        public async Task<NegocioDetalleDto> Handle(GetNegocioByCodigoEmpresaQuery query, CancellationToken cancellationToken)
        {
            var empresa = await _negocioRepository.GetByCodigoEmpresaAsync(query.CodigoEmpresa);

            if (empresa == null)
                return null;

            // Obtener productos de la empresa
            var productos = await _productoRepository.GetByEmpresaIdAsync(empresa.EmpresaID);

            // Obtener categorías que deben mostrarse (Mostrar = true y Activo = true)
            var categorias = await _categoriaRepository.GetByEmpresaIdMostrandoAsync(empresa.EmpresaID);

            var negocioDetalle = new NegocioDetalleDto
            {
                EmpresaID = empresa.EmpresaID,
                Nombre = empresa.Nombre,
                Descripcion = empresa.Descripcion,
                Email = empresa.Email,
                Telefono = empresa.Telefono,
                Direccion = empresa.Direccion,
                Referencia = empresa.Referencia,
                Categorias = categorias.Select(c => new CategoriaDetalleDto
                {
                    CategoriaID = c.CategoriaID,
                    EmpresaID = c.EmpresaID,
                    Descripcion = c.Descripcion,
                    Codigo = c.Codigo,
                    Activo = c.Activo,
                    Mostrar = c.Mostrar
                }).ToList(),
                Productos = productos.Select(p => new ProductoDetalleDto
                {
                    ProductoID = p.ProductoID,
                    EmpresaID = empresa.EmpresaID,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    URLImagen = p.Imagen
                }).ToList()
            };

            return await Task.FromResult(negocioDetalle);
        }
    }
}



