
using AutoMapper;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Domain.Entities;

namespace PedidosBarrio.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de Entidad a DTO
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.Ruc, opt => opt.MapFrom(src => src.Ruc))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AddressStreet, opt => opt.MapFrom(src => src.AddressStreet))
                .ForMember(dest => dest.AddressCity, opt => opt.MapFrom(src => src.AddressCity))
                .ForMember(dest => dest.AddressZipCode, opt => opt.MapFrom(src => src.AddressZipCode));

            // Mapeo de DTO a Entidad (para creación)
            CreateMap<CreateCompanyDto, Company>()
                .ForMember(dest => dest.Ruc, opt => opt.MapFrom(src => src.Ruc))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AddressStreet, opt => opt.MapFrom(src => src.AddressStreet))
                .ForMember(dest => dest.AddressCity, opt => opt.MapFrom(src => src.AddressCity))
                .ForMember(dest => dest.AddressZipCode, opt => opt.MapFrom(src => src.AddressZipCode));

        }
    }
}
