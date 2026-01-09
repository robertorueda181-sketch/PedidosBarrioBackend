using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Interfaces;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Mappers;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Application.Validator;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Common;
using PedidosBarrio.Infrastructure.Data.Repositories;
using System.Text;

namespace PedidosBarrio.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseProvider = configuration.GetSection("Database:Provider").Value ?? "SqlServer";

            // Crear el proveedor de base de datos según la configuración
            var dbProvider = DbConnectionProviderFactory.CreateFromString(databaseProvider, connectionString);
            services.AddSingleton<IDbConnectionProvider>(dbProvider);

            // Logger de aplicación
            services.AddScoped<IApplicationLogger>(sp => new ConsoleFileLogger("Logs"));

            // Repositorios
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ISuscripcionRepository, SuscripcionRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IImagenRepository, ImagenRepository>();
            services.AddScoped<ITipoRepository, TipoRepository>();
            services.AddScoped<IInmuebleRepository, InmuebleRepository>();
            services.AddScoped<INegocioRepository, NegocioRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();

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
            services.AddScoped<IValidator<CreateUsuarioDto>, CreateUsuarioDtoValidator>();
            services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
            services.AddScoped<IValidator<CreateSuscripcionDto>, CreateSuscripcionDtoValidator>();
            services.AddScoped<IValidator<CreateProductoDto>, CreateProductoDtoValidator>();
            services.AddScoped<IValidator<CreateImagenDto>, CreateImagenDtoValidator>();
            services.AddScoped<IValidator<CreateInmuebleDto>, CreateInmuebleDtoValidator>();
            services.AddScoped<IValidator<CreateNegocioDto>, CreateNegocioDtoValidator>();
            services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();

            // JWT Token Service
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CompanyService).Assembly));

            // El servicio de aplicación ahora es un "Facade" que usa MediatR
            services.AddScoped<ICompanyService, CompanyService>();

            // Configurar autenticación JWT
            var jwtSecret = configuration.GetSection("Jwt:Secret").Value;
            if (!string.IsNullOrEmpty(jwtSecret))
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = securityKey,
                            ValidateIssuer = true,
                            ValidIssuer = configuration.GetSection("Jwt:Issuer").Value ?? "PedidosBarrio",
                            ValidateAudience = true,
                            ValidAudience = configuration.GetSection("Jwt:Audience").Value ?? "PedidosBarrioApp",
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };
                    });
            }

            return services;
        }
    }
}

