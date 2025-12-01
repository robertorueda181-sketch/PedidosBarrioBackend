# ?? QUICK START GUIDE

## ? Inicio Rápido (5 minutos)

### Paso 1: Base de Datos (2 min)
```sql
-- Ejecutar en SQL Server Management Studio (SSMS)
-- Archivo: SQL/CreateDatabase.sql
-- Crea: BD, 7 tablas, índices, datos de prueba
```

### Paso 2: Stored Procedures (1 min)
```sql
-- Ejecutar en SSMS
-- Archivo: SQL/StoredProcedures.sql
-- Crea: 45+ procedimientos almacenados
```

### Paso 3: Configurar Connection String (1 min)
```json
// Archivo: PedidosBarrio/appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=GestionEmpresas;Integrated Security=true;TrustServerCertificate=True;"
  }
}
```

### Paso 4: Ejecutar (1 min)
```bash
cd PedidosBarrio
dotnet run
```

### Paso 5: Probar API
```
?? Swagger: https://localhost:7045/swagger
```

---

## ?? Archivos Principales

| Archivo | Propósito |
|---------|-----------|
| `Program.cs` | Configuración principal |
| `appsettings.json` | Configuración general |
| `appsettings.Development.json` | Configuración desarrollo |
| `DependencyInjection.cs` | IoC Container |

## ?? Estructura Carpetas

```
PedidosBarrio/
??? PedidosBarrio/           # API (Endpoints, Program.cs)
??? PedidosBarrio.Domain/    # Entidades e interfaces
??? PedidosBarrio.Application/  # CQRS, DTOs, Mappers
??? PedidosBarrio.Infrastructure/ # Repositorios
??? SQL/                     # Scripts SQL
??? PedidosBarrio.Tests/     # Tests
```

## ?? Comandos Útiles

```bash
# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run

# Tests
dotnet test

# Limpiar
dotnet clean
```

## ?? Endpoints Disponibles

```
GET    /api/Empresas              # Todas las empresas
GET    /api/Empresas/{id}         # Empresa por ID
POST   /api/Empresas              # Crear empresa
PUT    /api/Empresas/{id}         # Actualizar
DELETE /api/Empresas/{id}         # Eliminar

GET    /api/Suscripciones         # Todas
GET    /api/Productos             # Todas
GET    /api/Imagenes              # Todas
GET    /api/Tipos                 # Todos (lectura)
GET    /api/Inmuebles             # Todos
GET    /api/Negocios              # Todos
```

## ?? Crear Empresa (Ejemplo)

```bash
curl -X POST https://localhost:7045/api/Empresas \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Mi Empresa",
    "email": "info@empresa.com",
    "contrasena": "hash123",
    "telefono": "+34 123456789"
  }'
```

## ? Troubleshooting

**Error: Database not found**
? Ejecutar `SQL/CreateDatabase.sql` en SSMS

**Error: Connection timeout**
? Revisar Connection String en `appsettings.json`

**Error: Procedure not found**
? Ejecutar `SQL/StoredProcedures.sql` en SSMS

**Compilación error**
? `dotnet clean` y `dotnet restore`

## ?? Documentación

- **README.md** - Overview general
- **INSTALLATION_GUIDE.md** - Instalación detallada
- **API_EXAMPLES.md** - Ejemplos de requests
- **IMPLEMENTATION_SUMMARY.md** - Resumen técnico
- **EXECUTIVE_SUMMARY.md** - Resumen ejecutivo

## ? Verificación

Después de ejecutar, verifica:
- [ ] Swagger accesible en https://localhost:7045/swagger
- [ ] GET /api/Empresas retorna 200
- [ ] POST /api/Empresas crea nuevo registro
- [ ] PUT /api/Empresas/{id} actualiza
- [ ] DELETE /api/Empresas/{id} elimina

¡Listo! ??
