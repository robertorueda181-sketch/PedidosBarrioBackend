# ?? GUÍA DE INSTALACIÓN Y EJECUCIÓN

## ?? Requisitos Previos

- Visual Studio 2022 o superior
- .NET 10 SDK
- SQL Server 2019 o superior
- Git

## ?? PASO 1: Preparar la Base de Datos

### 1.1 Crear la Base de Datos y Tablas

1. Abre **SQL Server Management Studio (SSMS)**
2. Conéctate al servidor SQL Server
3. Abre un nueva consulta y ejecuta el script:
   ```
   SQL/CreateDatabase.sql
   ```
   Este script creará:
   - Base de datos `GestionEmpresas`
   - Todas las tablas necesarias
   - Índices
   - Datos de prueba

### 1.2 Crear los Stored Procedures

1. En una nueva consulta, ejecuta:
   ```
   SQL/StoredProcedures.sql
   ```
   Esto creará todos los procedimientos almacenados necesarios.

## ?? PASO 2: Configurar la Connection String

### 2.1 Editar appsettings.json

Abre `PedidosBarrio/appsettings.json` y configura:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=GestionEmpresas;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Reemplaza:**
- `YOUR_SERVER` - Nombre del servidor SQL Server (ej: localhost, .\SQLEXPRESS)
- `YOUR_PASSWORD` - Contraseña del usuario sa

### 2.2 Editar appsettings.Development.json (opcional)

Para desarrollo, puedes usar:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=GestionEmpresas;Integrated Security=true;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

## ?? PASO 3: Compilar y Ejecutar

### 3.1 Restaurar paquetes NuGet

```bash
cd PedidosBarrio
dotnet restore
```

### 3.2 Compilar la solución

```bash
dotnet build
```

Si hay errores, verifica que:
- .NET 10 está instalado
- Todos los paquetes NuGet están correctamente restaurados

### 3.3 Ejecutar la aplicación

```bash
dotnet run
```

O desde Visual Studio:
1. Click derecho en el proyecto `PedidosBarrio`
2. Selecciona "Set as Startup Project"
3. Presiona `F5` o click en "Start"

## ?? PASO 4: Acceder a la API

Una vez ejecutada, la aplicación estará disponible en:

- **API Base URL**: `https://localhost:7045`
- **Swagger UI**: `https://localhost:7045/swagger`
- **OpenAPI JSON**: `https://localhost:7045/swagger/v1/swagger.json`

## ?? PASO 5: Probar los Endpoints

### 5.1 Usando Swagger UI

1. Abre `https://localhost:7045/swagger` en tu navegador
2. Expande cualquier endpoint
3. Click en "Try it out"
4. Configura los parámetros
5. Click en "Execute"

### 5.2 Crear una Empresa (Ejemplo)

**Endpoint:**
```
POST /api/Empresas
```

**Body:**
```json
{
  "nombre": "Mi Nueva Empresa",
  "email": "contacto@empresa.com",
  "contrasena": "hash_seguro_123",
  "telefono": "+34 987654321"
}
```

**Respuesta exitosa (201 Created):**
```json
{
  "empresaID": 2,
  "nombre": "Mi Nueva Empresa",
  "email": "contacto@empresa.com",
  "telefono": "+34 987654321",
  "fechaRegistro": "2024-01-15T14:30:45.123Z",
  "activa": true
}
```

### 5.3 Obtener Todas las Empresas

**Endpoint:**
```
GET /api/Empresas
```

**Respuesta exitosa (200 OK):**
```json
[
  {
    "empresaID": 1,
    "nombre": "Empresa Test",
    "email": "test@empresa.com",
    "telefono": "+34 123456789",
    "fechaRegistro": "2024-01-15T10:00:00Z",
    "activa": true
  },
  {
    "empresaID": 2,
    "nombre": "Mi Nueva Empresa",
    "email": "contacto@empresa.com",
    "telefono": "+34 987654321",
    "fechaRegistro": "2024-01-15T14:30:45.123Z",
    "activa": true
  }
]
```

## ??? TROUBLESHOOTING

### Error: "Unable to resolve service for type IEmpresaRepository"

**Solución:**
1. Verifica que `DependencyInjection.cs` registra todos los repositorios
2. Limpia y reconstruye la solución
3. Elimina la carpeta `bin` y `obj`

### Error: "Connection timeout"

**Solución:**
1. Verifica que SQL Server está ejecutándose
2. Verifica la Connection String en `appsettings.json`
3. Verifica credenciales (usuario/contraseña)
4. Agrega `TrustServerCertificate=True;` si es necesario

### Error: "Login failed for user 'sa'"

**Solución:**
1. Verifica que el usuario `sa` existe en SQL Server
2. Verifica la contraseña configurada
3. Habilita la autenticación SQL Server en SQL Server Management Studio

### Error: "Database 'GestionEmpresas' does not exist"

**Solución:**
1. Ejecuta nuevamente el script `SQL/CreateDatabase.sql`
2. Verifica el nombre de la base de datos en la Connection String
3. Verifica que el script se ejecutó sin errores

## ?? Recursos Útiles

- [Microsoft Docs - MediatR](https://docs.microsoft.com/en-us/archive/msdn-magazine/2019/february/design-patterns-cqrs-in-the-cloud)
- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [AutoMapper Docs](https://automapper.org/)
- [FluentValidation](https://fluentvalidation.net/)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)

## ? Verificación Final

Si llegaste hasta aquí sin errores, ¡tu aplicación está lista para usar! 

Verifica que:
- [ ] La aplicación compila sin errores
- [ ] Puedes acceder a Swagger en `https://localhost:7045/swagger`
- [ ] Puedes crear una empresa desde Swagger
- [ ] Puedes obtener todas las empresas
- [ ] Puedes actualizar una empresa
- [ ] Puedes eliminar una empresa

¡Felicidades! ??
