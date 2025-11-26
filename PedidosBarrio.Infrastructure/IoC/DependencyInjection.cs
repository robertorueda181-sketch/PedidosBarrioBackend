using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton(configuration.GetConnectionString("DefaultConnection"));

            // Repositorios
            services.AddScoped<ICompanyRepository, CompanyRepository>(sp => // IEmpresaRepository -> ICompanyRepository, EmpresaRepository -> CompanyRepository
                new CompanyRepository(configuration.GetConnectionString("DefaultConnection")));

            // AutoMapper
            services.AddAutoMapper(cfg =>
            {
                // Aquí podrías agregar configuraciones globales de AutoMapper si las tuvieras
                // Por ejemplo: cfg.ValidateContentsOnStart = false;
            }, typeof(MappingProfile).Assembly);

            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<CreateCompanyDtoValidator>(); // CreateEmpresaDtoValidator -> CreateCompanyDtoValidator

            // MediatR
            // Registra MediatR y todos los handlers, requests y notificaciones
            // que se encuentren en el ensamblado de SolutionName.Application
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CompanyService).Assembly)); // EmpresaService -> CompanyService
            // Alternativamente, puedes usar el ensamblado de cualquier Command/Query o Handler
            // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCompanyCommand).Assembly));

            // El servicio de aplicación ahora es un "Facade" que usa MediatR
            services.AddScoped<ICompanyService, CompanyService>(); // IEmpresaService -> ICompanyService, EmpresaService -> CompanyService
            services.AddScoped<ICompanyRepository, CompanyRepository>(); // IEmpresaService -> ICompanyService, EmpresaService -> CompanyService

            return services;
        }
    }
}
