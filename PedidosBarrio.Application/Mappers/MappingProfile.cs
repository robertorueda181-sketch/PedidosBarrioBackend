using AutoMapper;
using PedidosBarrio.Application.Commands.ClienteAuth;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ===================== COMPANY MAPPINGS =====================
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.Ruc, opt => opt.MapFrom(src => src.Ruc))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AddressStreet, opt => opt.MapFrom(src => src.AddressStreet))
                .ForMember(dest => dest.AddressCity, opt => opt.MapFrom(src => src.AddressCity))
                .ForMember(dest => dest.AddressZipCode, opt => opt.MapFrom(src => src.AddressZipCode));

            CreateMap<CreateCompanyDto, Company>()
                .ForMember(dest => dest.Ruc, opt => opt.MapFrom(src => src.Ruc))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AddressStreet, opt => opt.MapFrom(src => src.AddressStreet))
                .ForMember(dest => dest.AddressCity, opt => opt.MapFrom(src => src.AddressCity))
                .ForMember(dest => dest.AddressZipCode, opt => opt.MapFrom(src => src.AddressZipCode));

            // ===================== EMPRESA MAPPINGS =====================

            CreateMap<CreateEmpresaDto, Empresa>();

            // ===================== USUARIO MAPPINGS =====================
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.UsuarioID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Activa));
            CreateMap<CreateUsuarioDto, Usuario>();

            // ===================== SUSCRIPCION MAPPINGS =====================
            CreateMap<Suscripcion, SuscripcionDto>();
            CreateMap<CreateSuscripcionDto, Suscripcion>();

            // ===================== PRODUCTO MAPPINGS =====================
            CreateMap<Producto, ProductoDto>()
                .ForMember(dest => dest.Precios, opt => opt.MapFrom(src => src.Presentaciones.SelectMany(p => p.Precios)));
            CreateMap<CreateProductoDto, Producto>();

            CreateMap<Presentacion, PresentacionDto>();
            CreateMap<Precio, PrecioDto>()
                .ForMember(dest => dest.PrecioValor, opt => opt.MapFrom(src => src.PrecioValor))
                .ForMember(dest => dest.EsPrincipal, opt => opt.MapFrom(src => src.Principal));

            // ===================== IMAGEN MAPPINGS =====================
            CreateMap<Imagen, ImagenDto>();
            CreateMap<Imagen, ImagenUrlDto>()
                .ForMember(dest => dest.URLImagen, opt => opt.MapFrom(src => src.Urlimagen))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion));
            CreateMap<CreateImagenDto, Imagen>();

            // ===================== TIPO MAPPINGS =====================
            CreateMap<Tipo, TipoDto>();

            // ===================== INMUEBLE MAPPINGS =====================
            CreateMap<Inmueble, InmuebleDto>();
            CreateMap<CreateInmuebleDto, Inmueble>();

            // Mapping para DTO con detalles (includes Tipos e Imagenes)
            CreateMap<InmuebleDetailsDto, InmuebleDetailsDto>();

            // ===================== NEGOCIO MAPPINGS =====================
            CreateMap<Negocio, NegocioDto>()
                .ForMember(dest => dest.UrlImagen, opt => opt.MapFrom(src => src.Imagenes.Urlimagen));
            CreateMap<CreateNegocioDto, Negocio>();

                // ===================== PASO INICIAL MAPPINGS =====================
                CreateMap<PasoInicial, PasoInicialDto>();

                    // ===================== CLIENTE AUTH MAPPINGS =====================
                    CreateMap<ClienteRegistroDto, ClienteGoogleAuthCommand>()
                        .ForMember(dest => dest.IdToken, opt => opt.MapFrom(src => src.IdToken))
                        .ForMember(dest => dest.DNI, opt => opt.MapFrom(src => src.DNI))
                        .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.Telefono))
                        .ForMember(dest => dest.Latitud, opt => opt.MapFrom(src => src.Latitud))
                        .ForMember(dest => dest.Longitud, opt => opt.MapFrom(src => src.Longitud));

                    // ===================== CLIENTE DIRECCION MAPPINGS =====================
                    CreateMap<ClienteDireccion, ClienteDireccionDto>();
                    CreateMap<CreateClienteDireccionDto, ClienteDireccion>()
                        .ForMember(dest => dest.ClienteDireccionID, opt => opt.MapFrom(_ => Guid.NewGuid()))
                        .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(_ => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)))
                        .ForMember(dest => dest.Activa, opt => opt.MapFrom(_ => true));
                    CreateMap<UpdateClienteDireccionDto, ClienteDireccion>()
                        .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(_ => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)))
                        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

                    // ===================== PAGINA MAPPINGS =====================
                    CreateMap<Pagina, PaginaDto>();
                    CreateMap<CreatePaginaDto, Pagina>()
                        .ForMember(dest => dest.PaginaID, opt => opt.MapFrom(_ => Guid.NewGuid()))
                        .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(_ => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)))
                        .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(_ => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)))
                        .ForMember(dest => dest.Activa, opt => opt.MapFrom(_ => true));
                    CreateMap<UpdatePaginaDto, Pagina>()
                        .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(_ => DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)))
                        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
                    CreateMap<UpdatePaginaDto, Pagina>()
                        .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(_ => DateTime.UtcNow))
                        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
                }
    }
}
