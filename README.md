# PedidosBarrio - API REST CRUD Completo

## Descripción General

Esta es una API REST completa usando **MediatR**, **Dapper** y **Minimal APIs** de ASP.NET Core 10. La arquitectura sigue el patrón CQRS con Mediator, separando Commands (escritura) y Queries (lectura).

## Estructura de Carpetas

```
PedidosBarrio/
??? PedidosBarrio.Domain/           # Capa de Dominio (Entidades, Interfaces)
??? PedidosBarrio.Application/      # Capa de Aplicación (Commands, Queries, DTOs, Mappers)
??? PedidosBarrio.Infrastructure/   # Capa de Infraestructura (Repositorios, IoC)
??? PedidosBarrio/                  # API (Endpoints, Program.cs)
??? SQL/                            # Scripts SQL
??? PedidosBarrio.Tests/            # Pruebas Unitarias
```

## Tecnologías Utilizadas

- **.NET 10**
- **C# 14.0**
- **MediatR** - Implementación del patrón Mediator
- **Dapper** - ORM ligero para consultas SQL
- **AutoMapper** - Mapeo entre entidades y DTOs
- **FluentValidation** - Validación de datos
- **Swagger/OpenAPI** - Documentación interactiva de la API

## Entidades Disponibles

1. **Empresa** - Gestión de empresas
2. **Suscripción** - Suscripciones de empresas
3. **Producto** - Productos de las empresas
4. **Imagen** - Imágenes de los productos
5. **Tipo** - Tipos de categorías (solo lectura)
6. **Inmueble** - Registros de inmuebles
7. **Negocio** - Registros de negocios

## Configuración Base de Datos

### Connection String

En `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SERVER_NAME;Database=GestionEmpresas;User Id=sa;Password=YOUR_PASSWORD;"
  }
}
```

### Crear Base de Datos y Tablas

1. Ejecutar el script de creación de tablas
2. Ejecutar los Stored Procedures desde `SQL/StoredProcedures.sql`
3. Ejecutar el script de actualización `SQL/AlterTable_Tipos.sql` si es necesario

## Endpoints Disponibles

### EMPRESAS
- `GET /api/Empresas` - Obtener todas las empresas
- `GET /api/Empresas/{id}` - Obtener una empresa por ID
- `POST /api/Empresas` - Crear nueva empresa
- `PUT /api/Empresas/{id}` - Actualizar empresa
- `DELETE /api/Empresas/{id}` - Eliminar empresa (soft delete)

### SUSCRIPCIONES
- `GET /api/Suscripciones` - Obtener todas las suscripciones
- `GET /api/Suscripciones/{id}` - Obtener suscripción por ID
- `GET /api/Suscripciones/empresa/{empresaId}` - Obtener suscripciones por empresa
- `POST /api/Suscripciones` - Crear nueva suscripción
- `PUT /api/Suscripciones/{id}` - Actualizar suscripción
- `DELETE /api/Suscripciones/{id}` - Eliminar suscripción (soft delete)

### PRODUCTOS
- `GET /api/Productos` - Obtener todos los productos
- `GET /api/Productos/{id}` - Obtener producto por ID
- `GET /api/Productos/empresa/{empresaId}` - Obtener productos por empresa
- `POST /api/Productos` - Crear nuevo producto
- `PUT /api/Productos/{id}` - Actualizar producto
- `DELETE /api/Productos/{id}` - Eliminar producto

### IMÁGENES
- `GET /api/Imagenes` - Obtener todas las imágenes
- `GET /api/Imagenes/{id}` - Obtener imagen por ID
- `GET /api/Imagenes/producto/{productoId}` - Obtener imágenes por producto
- `POST /api/Imagenes` - Crear nueva imagen
- `PUT /api/Imagenes/{id}` - Actualizar imagen
- `DELETE /api/Imagenes/{id}` - Eliminar imagen

### TIPOS (Solo Lectura)
- `GET /api/Tipos` - Obtener todos los tipos

### INMUEBLES
- `GET /api/Inmuebles` - Obtener todos los inmuebles
- `GET /api/Inmuebles/{id}` - Obtener inmueble por ID
- `GET /api/Inmuebles/empresa/{empresaId}` - Obtener inmuebles por empresa
- `POST /api/Inmuebles` - Crear nuevo inmueble
- `PUT /api/Inmuebles/{id}` - Actualizar inmueble
- `DELETE /api/Inmuebles/{id}` - Eliminar inmueble

### NEGOCIOS
- `GET /api/Negocios` - Obtener todos los negocios
- `GET /api/Negocios/{id}` - Obtener negocio por ID
- `GET /api/Negocios/empresa/{empresaId}` - Obtener negocios por empresa
- `POST /api/Negocios` - Crear nuevo negocio
- `PUT /api/Negocios/{id}` - Actualizar negocio
- `DELETE /api/Negocios/{id}` - Eliminar negocio

## Ejemplo de Uso

### Crear una Empresa
```bash
POST /api/Empresas
Content-Type: application/json

{
  "nombre": "Mi Empresa",
  "email": "info@miempresa.com",
  "contrasena": "contraseña_hash",
  "telefono": "+34 123456789"
}
```

### Respuesta
```json
{
  "empresaID": 1,
  "nombre": "Mi Empresa",
  "email": "info@miempresa.com",
  "telefono": "+34 123456789",
  "fechaRegistro": "2024-01-15T10:30:00Z",
  "activa": true
}
```

## Patrones de Arquitectura

### CQRS (Command Query Responsibility Segregation)
- **Commands**: Operaciones de escritura (Create, Update, Delete)
- **Queries**: Operaciones de lectura (Get, GetAll)

### Mediator Pattern
Usa MediatR para desacoplar los endpoints de la lógica de negocio.

### Repository Pattern
Abstracción de acceso a datos a través de interfaces.

### Dependency Injection
Inyección de dependencias en IoC Container.

## Validación

Se utiliza **FluentValidation** para validar los DTOs en los Commands. 

Crear un validador:
```csharp
public class CreateEmpresaDtoValidator : AbstractValidator<CreateEmpresaDto>
{
    public CreateEmpresaDtoValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es requerido");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email inválido");
    }
}
```

## Mapeo de Entidades (AutoMapper)

Los mapeos están configurados en `PedidosBarrio.Application/Mappers/MappingProfile.cs`

## Stored Procedures

Todos los repositorios utilizan **SOLO Stored Procedures** de SQL Server.

- **Crear**: `sp_Create{Entidad}`
- **Leer**: `sp_Get{Entidad}ById` / `sp_GetAll{Entidades}`
- **Actualizar**: `sp_Update{Entidad}`
- **Eliminar**: `sp_Delete{Entidad}` (Soft delete para Empresa y Suscripción)

## Eliminación (Soft Delete)

Para **Empresa** y **Suscripción**: Se marca la columna `Activa = 0`
Para otras entidades: Se eliminan directamente de la base de datos

## Ejecución

1. Restaurar/compilar el proyecto:
```bash
dotnet restore
dotnet build
```

2. Ejecutar las migraciones (si las hay) y crear las tablas
3. Ejecutar la aplicación:
```bash
dotnet run
```

4. Acceder a Swagger en: `https://localhost:7045/swagger`

## Contribuciones

Este proyecto es parte de PedidosBarrio. Para contribuir, crear un branch, hacer cambios y enviar pull requests.

## Licencia

MIT License
