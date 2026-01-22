using Microsoft.OpenApi;
using PedidosBarrio.Api.EndPoint;
using PedidosBarrio.Api.Middlewares;
using PedidosBarrio.Infrastructure.IoC;
using PedidosBarrio.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS desde appsettings
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
if (corsOrigins != null && corsOrigins.Length > 0)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular", policy =>
        {
            policy.WithOrigins(corsOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });
}

// ¡Llama a tu método de extensión aquí!
// Aquí llamas a tu método de extensión
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("swagger/openapi/v1.json");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Apunta al endpoint correcto de tu documento Swagger/OpenAPI
        // Si AddSwaggerGen usa "v1", entonces aquí también debe ser "v1"
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API de Empresas v1");
        c.RoutePrefix = "swagger"; // La URL base para Swagger UI (ej. https://localhost:7045/swagger)
    });
}

app.UseHttpsRedirection();

// Servir archivos estáticos desde wwwroot (default)
app.UseStaticFiles();

// Opcional: Servir también desde una carpeta 'images' en el root si existe
var externalImagesPath = Path.Combine(builder.Environment.ContentRootPath, "images");
if (Directory.Exists(externalImagesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(externalImagesPath),
        RequestPath = "/images"
    });
}

// Usar CORS
app.UseCors("AllowAngular");

// Usar autenticación JWT
app.UseAuthentication();

// Usar el middleware de logging antes del error handling
app.UseLoggingMiddleware();
app.UseErrorHandlingMiddleware();

app.UseAuthorization();

app.MapControllers();

// Mapear todos los endpoints
app.MapRegisterEndpoints();
app.MapLoginEndpoints();
app.MapEmpresaEndpoints();
app.MapCategoriaEndpoints(); // Incluye endpoints de categorías Y productos
app.MapImagenEndpoints();
app.MapTipoEndpoints();
app.MapInmuebleEndpoints();
app.MapNegocioEndpoints();
app.MapSearchEndpoints();
app.MapConfiguracionEndpoints();

app.Run();
