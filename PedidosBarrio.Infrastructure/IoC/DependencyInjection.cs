using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PedidosBarrio.Application.Commands.ModerateText;
using PedidosBarrio.Application.Commands.RegisterSocial;
using PedidosBarrio.Application.Commands.ValidateImage;
using PedidosBarrio.Application.DTOs;
using PedidosBarrio.Application.Interfaces;
using PedidosBarrio.Application.Logging;
using PedidosBarrio.Application.Mappers;
using PedidosBarrio.Application.Services;
using PedidosBarrio.Application.Validator;
using PedidosBarrio.Domain.Enums;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Authorization;
using PedidosBarrio.Infrastructure.Data.Common;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories;
using PedidosBarrio.Infrastructure.Services;
using System.Text;

namespace PedidosBarrio.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // ===== ENTITY FRAMEWORK CORE =====
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<PedidosBarrioDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(30);
                });

                // Configuraciones adicionales para desarrollo
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                }
            });

            // ===== CONFIGURACIÓN DE BASE DE DATOS (DAPPER - LEGACY) =====
            var databaseProvider = configuration.GetSection("Database:Provider").Value ?? "SqlServer";

            // Crear el proveedor de base de datos según la configuración
            var dbProvider = DbConnectionProviderFactory.CreateFromString(databaseProvider, connectionString);
            services.AddSingleton<IDbConnectionProvider>(dbProvider);

            // Logger de aplicación
            services.AddScoped<IApplicationLogger>(sp => new ConsoleFileLogger("Logs"));

            // HttpContextAccessor para obtener información del usuario logueado
            services.AddHttpContextAccessor();

            // Current User Service para obtener información del token
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Image Moderation Service - Google Vision API
            services.AddHttpClient<IImageModerationService, GoogleVisionImageModerationService>();

            // Memory Cache para moderación de texto
            services.AddMemoryCache();

            // Text Moderation Service - Cadena de servicios
            services.AddHttpClient<OpenAITextModerationService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30); // Timeout de 30 segundos
                client.DefaultRequestHeaders.Add("User-Agent", "PedidosBarrio/1.0");
            });

            // Registrar servicios en cadena: OpenAI -> Cache -> Hybrid
            services.AddScoped<CachedTextModerationService>();
            services.AddScoped<ITextModerationService, HybridTextModerationService>();

            // Log Repository
            services.AddScoped<ILogRepository, LogRepository>();

            // Database Logger Service
            services.AddScoped<IApplicationLogger, DatabaseLogger>();

            // Image Processing Service
            services.AddScoped<IImageProcessingService, ImageProcessingService>();

            // Email Service
            services.AddScoped<IEmailService, SmtpEmailService>();

            // Subscription Authorization Service (temporal sin ISuscripcionRepository)
            services.AddScoped<ISubscriptionAuthorizationService, SubscriptionAuthorizationService>();

            // Repositorios
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ISuscripcionRepository, SuscripcionRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IPrecioRepository, PrecioRepository>();
            services.AddScoped<IImagenRepository, ImagenRepository>();
            services.AddScoped<ITipoRepository, TipoRepository>();
            services.AddScoped<IInmuebleRepository, InmuebleRepository>();
            services.AddScoped<INegocioRepository, NegocioRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<ISearchRepository, SearchRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            
            // Unit of Work para transacciones
            services.AddScoped<IUnitOfWork, UnitOfWork>();

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
            services.AddScoped<IValidator<RegisterSocialRequestDto>, RegisterSocialRequestValidator>();
            services.AddScoped<IValidator<CreateCategoriaDto>, CreateCategoriaDtoValidator>();
            services.AddScoped<IValidator<UpdateCategoriaDto>, UpdateCategoriaDtoValidator>();
            services.AddScoped<IValidator<ImageValidationRequestDto>, ImageValidationRequestDtoValidator>();
            services.AddScoped<IValidator<TextModerationRequestDto>, TextModerationRequestDtoValidator>();
            services.AddScoped<IValidator<CreateProductoDto>, CreateProductoDtoValidator>();
            services.AddScoped<IValidator<UpdateProductoDto>, UpdateProductoDtoValidator>();
            
            // Command Validators
            services.AddScoped<IValidator<RegisterSocialCommand>, RegisterSocialCommandValidator>();
            services.AddScoped<IValidator<ModerateTextCommand>, ModerateTextCommandValidator>();
            services.AddScoped<IValidator<ValidateImageCommand>, ValidateImageCommandValidator>();

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

            // ===== CONFIGURAR AUTORIZACIÓN PERSONALIZADA (TEMPORAL) =====
            services.AddAuthorization(options =>
            {
                // Políticas básicas por suscripción
                options.AddPolicy(AuthorizationPolicies.RequireFreePlan, policy =>
                    policy.Requirements.Add(new SubscriptionRequirement(TipoSuscripcion.Free)));
                
                options.AddPolicy(AuthorizationPolicies.RequireVecinoPlan, policy =>
                    policy.Requirements.Add(new SubscriptionRequirement(TipoSuscripcion.Vecino)));
                
                options.AddPolicy(AuthorizationPolicies.RequireEmpresaPlan, policy =>
                    policy.Requirements.Add(new SubscriptionRequirement(TipoSuscripcion.Empresa)));

                // Políticas por rol
                options.AddPolicy(AuthorizationPolicies.RequireAdmin, policy =>
                    policy.Requirements.Add(new RoleRequirement(UsuarioRol.Admin)));

                // Feature access policies (temporales)
                options.AddPolicy(AuthorizationPolicies.FeatureAccess.CreateCategories, policy =>
                    policy.Requirements.Add(new SubscriptionOrRoleRequirement(TipoSuscripcion.Empresa, UsuarioRol.Admin)));
                
                options.AddPolicy(AuthorizationPolicies.FeatureAccess.ModerateImages, policy =>
                    policy.Requirements.Add(new SubscriptionOrRoleRequirement(TipoSuscripcion.Vecino, UsuarioRol.Moderador)));
                
                options.AddPolicy(AuthorizationPolicies.FeatureAccess.ModerateText, policy =>
                    policy.Requirements.Add(new SubscriptionOrRoleRequirement(TipoSuscripcion.Free, UsuarioRol.Moderador)));
                
                options.AddPolicy(AuthorizationPolicies.FeatureAccess.SendEmails, policy =>
                    policy.Requirements.Add(new SubscriptionOrRoleRequirement(TipoSuscripcion.Empresa, UsuarioRol.Admin)));
            });

            // Registrar Authorization Handlers
            services.AddScoped<IAuthorizationHandler, SubscriptionAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, SubscriptionOrRoleAuthorizationHandler>();

            return services;
        }
    }
}

