using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Interfaces;
using PedidosBarrio.Application.Mappers;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Application.Validator;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Repositories;

namespace PedidosBarrio.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton(connectionString);

            // Repositorios
            services.AddScoped<ICompanyRepository, CompanyRepository>(sp => 
                new CompanyRepository(connectionString));
            services.AddScoped<IEmpresaRepository, EmpresaRepository>(sp => 
                new EmpresaRepository(connectionString));
            services.AddScoped<ISuscripcionRepository, SuscripcionRepository>(sp => 
                new SuscripcionRepository(connectionString));
            services.AddScoped<IProductoRepository, ProductoRepository>(sp => 
                new ProductoRepository(connectionString));
            services.AddScoped<IImagenRepository, ImagenRepository>(sp => 
                new ImagenRepository(connectionString));
            services.AddScoped<ITipoRepository, TipoRepository>(sp => 
                new TipoRepository(connectionString));
            services.AddScoped<IInmuebleRepository, InmuebleRepository>(sp => 
                new InmuebleRepository(connectionString));
            services.AddScoped<INegocioRepository, NegocioRepository>(sp => 
                new NegocioRepository(connectionString));

            // AutoMapper
            services.AddAutoMapper(cfg =>
            {
                // Aquí podrías agregar configuraciones globales de AutoMapper si las tuvieras
                // Por ejemplo: cfg.ValidateContentsOnStart = false;
            }, typeof(MappingProfile).Assembly);

            // FluentValidation - Registrar todos los validadores del assembly
            services.AddValidatorsFromAssemblyContaining<CreateCompanyDtoValidator>();

            // Registrar validadores específicos explícitamente para asegurar que estén disponibles
            services.AddScoped<IValidator<CreateEmpresaDto>, CreateEmpresaDtoValidator>();
            services.AddScoped<IValidator<CreateSuscripcionDto>, CreateSuscripcionDtoValidator>();
            services.AddScoped<IValidator<CreateProductoDto>, CreateProductoDtoValidator>();
            services.AddScoped<IValidator<CreateImagenDto>, CreateImagenDtoValidator>();
            services.AddScoped<IValidator<CreateInmuebleDto>, CreateInmuebleDtoValidator>();
            services.AddScoped<IValidator<CreateNegocioDto>, CreateNegocioDtoValidator>();

            // MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CompanyService).Assembly));

            // El servicio de aplicación ahora es un "Facade" que usa MediatR
            services.AddScoped<ICompanyService, CompanyService>();

            return services;
        }
    }
}
