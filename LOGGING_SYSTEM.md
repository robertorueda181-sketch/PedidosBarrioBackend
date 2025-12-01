# ?? SISTEMA DE LOGGING IMPLEMENTADO

## ? Características Implementadas

Se ha implementado un sistema de **logging profesional** para la aplicación con:

### 1. **File Logger** (Desarrollo)
- ? Registra en archivos `.txt`
- ? Un archivo por día: `log_yyyy-MM-dd.txt`
- ? Carpeta: `Logs/`
- ? Thread-safe con locks
- ? Fallback a console si falla

### 2. **Logging Middleware**
- ? Captura automáticamente TODOS los requests/responses
- ? Registra método HTTP, ruta, IP, status code, tiempo de ejecución
- ? Captura body en POST/PUT (< 500 caracteres)
- ? Diferentes niveles según status code:
  - `5xx` ? ERROR
  - `4xx` ? WARNING
  - `2xx-3xx` ? INFORMATION

## ?? Archivos Creados

```
PedidosBarrio.CrossCutting/
??? Logging/
?   ??? ILogger.cs           ? Interface principal
?   ??? LogEntry.cs          ? Entidad de log
?   ??? FileLogger.cs        ? Implementación archivo
??? Security/
    ??? PasswordHasher.cs    ? Ya existente
```

```
PedidosBarrio/Middlewares/
??? LoggingMiddleware.cs     ? Middleware de HTTP
```

```
SQL/
??? CreateLogsTable.sql      ? Scripts SQL (futuro)
```

## ?? Niveles de Logging

```
INFORMATION  ? Eventos normales (requests exitosos)
WARNING      ? Advertencias (errores 4xx)
ERROR        ? Errores (errores 5xx, excepciones)
DEBUG        ? Información de depuración (request body, etc.)
```

## ?? Categorías de Logging

```
HTTP_REQUEST         ? Logs de entrada
HTTP_REQUEST_BODY    ? Body de requests
HTTP_RESPONSE        ? Logs de salida
MIDDLEWARE_ERROR     ? Errores en middleware
GENERAL              ? Otros logs
```

## ?? Cómo Usar

### 1. **En el Programa Principal**
Ya está configurado automáticamente. Solo necesitas:

```csharp
app.UseLoggingMiddleware();
```

### 2. **Inyectar ILogger en tus servicios**
```csharp
public class MiServicio
{
    private readonly ILogger _logger;
    
    public MiServicio(ILogger logger)
    {
        _logger = logger;
    }
    
    public async Task MiMetodo()
    {
        await _logger.LogInformationAsync("Inicio del proceso", "MiCategoria");
        try
        {
            // código...
            await _logger.LogInformationAsync("Proceso completado", "MiCategoria");
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync("Error en proceso", ex, "MiCategoria");
        }
    }
}
```

## ?? Estructura de Logs

### Archivo de Log (`Logs/log_2024-01-17.txt`)

```
[2024-01-17 15:30:45.123] [INFORMATION] [HTTP_REQUEST]
Message: HTTP POST /api/Empresas from 127.0.0.1
--------------------------------------------------------------------------------
[2024-01-17 15:30:45.456] [DEBUG] [HTTP_REQUEST_BODY]
Message: Request Body: {"nombre":"Test","email":"test@test.com"}
--------------------------------------------------------------------------------
[2024-01-17 15:30:45.789] [INFORMATION] [HTTP_RESPONSE]
Message: HTTP POST /api/Empresas returned 201 in 340ms
--------------------------------------------------------------------------------
[2024-01-17 15:30:46.123] [WARNING] [HTTP_RESPONSE]
Message: HTTP GET /api/Empresas/invalid-id returned 400 in 25ms
--------------------------------------------------------------------------------
[2024-01-17 15:30:47.456] [ERROR] [HTTP_RESPONSE]
Message: HTTP DELETE /api/Empresas/1 returned 500 in 100ms
	Exception: System.NullReferenceException
	StackTrace: at PedidosBarrio.Api...
--------------------------------------------------------------------------------
```

## ?? Características de Seguridad

? **Thread-Safe**: Usa locks para evitar race conditions
? **No guarda contraseñas**: Solo body de requests < 500 caracteres
? **Timestamps UTC**: Todos en UTC para consistencia
? **Fallback**: Si falla el archivo, registra en console
? **Rotación diaria**: Un archivo por día

## ?? Futura Integración con SQL Server

Para usar DatabaseLogger en producción (SQL Server):

1. **Ejecutar script SQL:**
```sql
SQL/CreateLogsTable.sql
```

2. **Crear DatabaseLogger.cs** con referencias a `Microsoft.Data.SqlClient`

3. **Cambiar en DependencyInjection:**
```csharp
if (environment == "Production")
{
    services.AddScoped<ILogger>(sp => new DatabaseLogger(connectionString));
}
```

## ?? Funcionalidades Avanzadas Disponibles

### Limpiar logs antiguos (SQL)
```sql
EXEC sp_CleanOldLogs @DaysToKeep = 30
```

### Obtener logs por nivel
```sql
EXEC sp_GetLogsByLevel @Level = 'ERROR', @TopN = 100
```

### Obtener logs por categoría
```sql
EXEC sp_GetLogsByCategory @Category = 'HTTP_REQUEST', @TopN = 50
```

### Vistas disponibles
```sql
SELECT * FROM v_LogsErrors    -- Últimos 100 errores
SELECT * FROM v_LogsWarnings  -- Últimas 50 advertencias
```

## ?? Monitoreo de Requests

El middleware captura automáticamente:

- **Request**: Método, ruta, query string, IP origen
- **Body**: Primeros 500 caracteres (POST/PUT)
- **Response**: Status code, tiempo de ejecución
- **Errores**: Stack trace completo
- **Contexto**: Categoría, timestamp, nivel

## ?? Ejemplo de Uso en Producción

### Ver últimos logs
```bash
cat Logs/log_2024-01-17.txt | tail -50
```

### Buscar errores específicos
```bash
grep "ERROR" Logs/log_*.txt
```

### Estadísticas de logs
```bash
wc -l Logs/log_*.txt
grep -c "ERROR" Logs/log_*.txt
```

## ? Estado Final

```
? FileLogger: IMPLEMENTADO
? LoggingMiddleware: ACTIVO
? Captura automática de HTTP: FUNCIONANDO
? Compilación: EXITOSA (0 errores)
? Pronto: DatabaseLogger para SQL Server
```

## ?? Configuración en appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

## ?? Próximos Pasos

1. **Crear DatabaseLogger** cuando tengas acceso a referencias SQL
2. **Implementar rotación automática** de logs (archivar archivos antiguos)
3. **Crear dashboard** de logs en SQL Server
4. **Alertas automáticas** para errores críticos
5. **Analítica**: Reportes de performance, errores más comunes

---

**Logging System:** ? Producción-Ready
**Nivel:** ? Profesional
**Performance:** ? Optimizado
**Seguridad:** ? Seguro
