# ?? CAMBIO: INT ? GUID para EmpresaID

## ? Cambio Completado

Se ha migrado `EmpresaID` de `INT` a `GUID` (UNIQUEIDENTIFIER) en SQL Server.

## ?? Comparativa

| Aspecto | INT | GUID ? |
|---------|-----|--------|
| **Rango** | 2.1 billones | Único globalmente |
| **Seguridad** | Predecible | No predecible |
| **Escalabilidad** | Una base de datos | Múltiples sistemas |
| **API URL** | `/api/empresas/123` | `/api/empresas/a1b2c3d4-e5f6-7890-abcd-ef1234567890` |
| **Exposición** | Revela cantidad total | Oculta datos sensibles |
| **Replicación** | Requiere servidor | Se genera localmente |
| **Estándar** | ? | ? Industria |

## ?? Archivos Modificados

### 1. **Domain Layer**
```
? Empresa.cs
   - EmpresaID: int ? Guid
   - Inicializa con Guid.NewGuid()

? IEmpresaRepository.cs
   - GetByIdAsync(int id) ? GetByIdAsync(Guid id)
   - DeleteAsync(int id) ? DeleteAsync(Guid id)
   - AddAsync() ? void (no retorna ID)
```

### 2. **Infrastructure Layer**
```
? EmpresaRepository.cs
   - Todos los métodos usan Guid
   - AddAsync no retorna ID (ya se genera en entidad)
```

### 3. **Application Layer**
```
? EmpresaDto.cs
   - EmpresaID: int ? Guid

? GetEmpresaByIdQuery.cs
   - EmpresaID: int ? Guid

? UpdateEmpresaCommand.cs
   - EmpresaID: int ? Guid

? DeleteEmpresaCommand.cs
   - EmpresaID: int ? Guid
```

### 4. **API Layer**
```
? EmpresaEndpoint.cs
   - {id:int} ? {id:guid}
   - Todas las rutas actualizadas
```

## ?? Archivos Nuevos (SQL)

```
SQL/
??? MigrationEmpresaToGUID.sql
?   ??? Script de migración de datos
?
??? StoredProcedures_Empresa_GUID.sql
    ??? SP actualizados con UNIQUEIDENTIFIER
```

## ??? Cambios en Base de Datos

**Antes:**
```sql
EmpresaID INT IDENTITY(1,1) PRIMARY KEY
```

**Después:**
```sql
EmpresaID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID()
```

### Stored Procedures Actualizados
- ? `sp_CreateEmpresa` - Recibe EmpresaID como UNIQUEIDENTIFIER
- ? `sp_GetEmpresaById` - Parámetro UNIQUEIDENTIFIER
- ? `sp_UpdateEmpresa` - Parámetro UNIQUEIDENTIFIER
- ? `sp_DeleteEmpresa` - Parámetro UNIQUEIDENTIFIER

### Relaciones Actualizadas
- ? Suscripciones.EmpresaID ? UNIQUEIDENTIFIER
- ? Productos.EmpresaID ? UNIQUEIDENTIFIER
- ? Inmuebles.EmpresaID ? UNIQUEIDENTIFIER
- ? Negocios.EmpresaID ? UNIQUEIDENTIFIER

## ?? Pasos para Aplicar el Cambio

### 1. Hacer backup de la BD
```sql
BACKUP DATABASE GestionEmpresas 
TO DISK = 'C:\backup\GestionEmpresas.bak'
```

### 2. Ejecutar migración
```sql
-- Ejecutar en este orden:
1. SQL/MigrationEmpresaToGUID.sql
2. SQL/StoredProcedures_Empresa_GUID.sql
```

### 3. Compilar la aplicación
```bash
dotnet clean
dotnet build
```

### 4. Ejecutar
```bash
dotnet run
```

## ?? Pruebas Sugeridas

### Test 1: Crear Empresa
```http
POST /api/Empresas
Content-Type: application/json

{
  "nombre": "Test Enterprise",
  "email": "test@empresa.com",
  "contrasena": "pass123",
  "telefono": "+34 123456789"
}
```

**Respuesta esperada (201):**
```json
{
  "empresaID": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "nombre": "Test Enterprise",
  "email": "test@empresa.com",
  "telefono": "+34 123456789",
  "fechaRegistro": "2024-01-16T15:30:00Z",
  "activa": true
}
```

### Test 2: Obtener Empresa por GUID
```http
GET /api/Empresas/a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

### Test 3: Actualizar Empresa
```http
PUT /api/Empresas/a1b2c3d4-e5f6-7890-abcd-ef1234567890
Content-Type: application/json

{
  "empresaID": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "nombre": "Test Enterprise Updated",
  "email": "updated@empresa.com",
  "telefono": "+34 987654321",
  "activa": true
}
```

### Test 4: Eliminar Empresa
```http
DELETE /api/Empresas/a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

## ? Beneficios Inmediatos

? **Seguridad Mejorada**
- No expone cantidad de empresas
- GUIDs son impredecibles

? **Mejor Escalabilidad**
- Se genera en cliente sin contactar BD
- Funciona en sistemas distribuidos

? **URLs más Profesionales**
- Cumple estándares RESTful
- Mejor para APIs públicas

? **Compatibilidad Futura**
- Listo para microservicios
- Facilita replicación de datos

## ?? Rollback (si es necesario)

Si necesitas volver a INT:
1. Restaurar backup anterior
2. Revertir cambios en código
3. Cambiar Guid ? int en todos los archivos

## ?? Estado Actual

```
? Código compilado: OK
? SQL Scripts: Listos
? Endpoints actualizados: OK
? DTOs actualizados: OK
? Repositorio actualizado: OK
```

## ?? Próximo Paso

**Ejecuta los scripts SQL en este orden:**
1. `SQL/MigrationEmpresaToGUID.sql` - Migra datos
2. `SQL/StoredProcedures_Empresa_GUID.sql` - Actualiza SP

---

**Estado:** ? **LISTO PARA DEPLOYMENT**
**Cambio:** INT ? GUID (UNIQUEIDENTIFIER)
**Impacto:** Solo tabla Empresas y referencias
