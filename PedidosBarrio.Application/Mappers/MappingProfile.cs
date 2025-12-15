using AutoMapper;
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
            CreateMap<Empresa, EmpresaDto>()
                 .ForMember(dest => dest.EmpresaID, opt => opt.MapFrom(src => src.ID));
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
            CreateMap<Producto, ProductoDto>();
            CreateMap<CreateProductoDto, Producto>();

            // ===================== IMAGEN MAPPINGS =====================
            CreateMap<Imagen, ImagenDto>();
            CreateMap<CreateImagenDto, Imagen>();

            // ===================== TIPO MAPPINGS =====================
            CreateMap<Tipo, TipoDto>();

            // ===================== INMUEBLE MAPPINGS =====================
            CreateMap<Inmueble, InmuebleDto>();
            CreateMap<CreateInmuebleDto, Inmueble>();

            // ===================== NEGOCIO MAPPINGS =====================
            CreateMap<Negocio, NegocioDto>();
            CreateMap<CreateNegocioDto, Negocio>();
        }
    }
}
