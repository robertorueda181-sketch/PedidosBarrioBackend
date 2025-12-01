# ?? INTEGRACIÓN: PasswordHasher PBKDF2

## ? Implementación Completada

Se ha integrado encriptación segura de contraseñas usando **PBKDF2 con SHA256** en la aplicación.

## ?? Características de Seguridad

### PBKDF2 (Password-Based Key Derivation Function 2)
```
? 100,000 iteraciones (NIST recomendado)
? SHA256 como algoritmo hash
? Salt aleatorio de 256 bits (32 bytes)
? Hash de 256 bits (32 bytes)
? Protección contra timing attacks
```

## ?? Archivos Creados

### 1. `PasswordHasher.cs`
```
PedidosBarrio.Application/Utilities/PasswordHasher.cs
```

**Métodos disponibles:**
```csharp
// Generar hash y salt
public static (string hash, string salt) HashPassword(string password)

// Verificar contraseña
public static bool VerifyPassword(string password, string storedHash, string storedSalt)
```

## ?? Flujo de Seguridad

### 1. **Creación de Empresa (Registro)**

```
Usuario ? CreateEmpresaDto (texto plano) 
    ?
Validación (8+ chars, mayús/minús, números, caracteres especiales)
    ?
CreateEmpresaCommand (contiene contraseña texto plano)
    ?
CreateEmpresaCommandHandler
    ?
PasswordHasher.HashPassword()
    ?? Genera salt aleatorio
    ?? Calcula hash con PBKDF2
    ?? Retorna (hash, salt)
    ?
Guardar en BD: ContrasenaHash + ContrasenaSalt
```

### 2. **Verificación de Contraseña (Login - Futuro)**

```
Usuario ? Contraseña (texto plano)
    ?
Obtener empresa por email
    ?
PasswordHasher.VerifyPassword(
    contraseña_ingresada,
    empresa.ContrasenaHash,
    empresa.ContrasenaSalt
)
    ?? Obtiene salt almacenado
    ?? Calcula hash con misma sal
    ?? Comparación segura (FixedTimeEquals)
    ?? Retorna: true/false
```

## ?? Cambios en la Entidad Empresa

### Antes
```csharp
public Empresa(string nombre, string email, string contrasena, string telefono)
```

### Después
```csharp
public Empresa(string nombre, string email, string contrasenaHash, string contrasenaSalt, string telefono)
{
    ID = Guid.NewGuid();
    Nombre = nombre;
    Email = email;
    ContrasenaHash = contrasenaHash;      // Hash (no texto plano)
    ContrasenaSalt = contrasenaSalt;      // Salt aleatorio
    Telefono = telefono;
}
```

## ?? Validación de Contraseña

Se actualizó `CreateEmpresaDtoValidator` con reglas más seguras:

? **Mínimo 8 caracteres** (antes: 6)
? **Mayúsculas y minúsculas requeridas**
? **Al menos un número**
? **Al menos un carácter especial**

Ejemplo de contraseña válida:
```
Valida1!      ? (8 chars, mayús, minús, número, especial)
pass123        ? (sin mayúsculas, sin especiales)
Password       ? (sin números, sin especiales)
```

## ?? Base de Datos

### Tabla Empresas
```sql
CREATE TABLE Empresas (
    ID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Nombre VARCHAR(255) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    ContrasenaHash VARCHAR(MAX) NOT NULL,    -- Base64 encoded hash
    ContrasenaSalt VARCHAR(MAX) NOT NULL,    -- Base64 encoded salt
    Telefono VARCHAR(20),
    Descripcion VARCHAR(MAX),
    Direccion VARCHAR(500),
    Referencia VARCHAR(255),
    FechaRegistro DATETIME DEFAULT GETDATE(),
    Activa BIT DEFAULT 1
);
```

## ?? Ejemplo de Uso

### Crear Empresa (POST)
```http
POST /api/Empresas
Content-Type: application/json

{
  "nombre": "Mi Empresa",
  "descripcion": "Descripción de la empresa",
  "direccion": "Calle Principal 123",
  "referencia": "Ref001",
  "email": "info@empresa.com",
  "contrasena": "MySecure123!",  // Texto plano - se encripta en handler
  "telefono": "+34 912345678"
}
```

### Respuesta
```json
{
  "empresaID": "550e8400-e29b-41d4-a716-446655440000",
  "nombre": "Mi Empresa",
  "descripcion": "Descripción de la empresa",
  "direccion": "Calle Principal 123",
  "referencia": "Ref001",
  "email": "info@empresa.com",
  "telefono": "+34 912345678",
  "fechaRegistro": "2024-01-17T15:30:00Z",
  "activa": true
}
```

**Base de Datos almacena:**
```
ContrasenaHash: "AY8RlF3dEzP8q9E+j2K+l3M="  (Base64)
ContrasenaSalt: "xK9P0qL+m8N-o1R2sT3uV4W="  (Base64)
```

## ??? Protecciones Implementadas

### 1. **Timing Attack Resistant**
```csharp
CryptographicOperations.FixedTimeEquals(
    Convert.FromBase64String(storedHash),
    hashBytes
);
```
Compara hashes en tiempo constante, evitando ataques por timing.

### 2. **Salt Aleatorio Único**
Cada contraseña genera un salt nuevo y único usando `RandomNumberGenerator.Create()`.

### 3. **100,000 Iteraciones PBKDF2**
Aumenta el tiempo de cálculo, haciendo ataques de fuerza bruta muy costosos.

### 4. **Nunca se transmite ni almacena en texto plano**
- El cliente envía solo en el registro (API sobre HTTPS)
- El servidor inmediatamente lo hashea
- Se almacena solo hash + salt

## ?? Requisitos SQL

Asegúrate de que la tabla Empresas tiene las columnas:

```sql
ALTER TABLE Empresas ADD ContrasenaHash VARCHAR(MAX) NOT NULL;
ALTER TABLE Empresas ADD ContrasenaSalt VARCHAR(MAX) NOT NULL;
```

## ?? Checklist de Seguridad

- [x] PasswordHasher implementado con PBKDF2
- [x] 100,000 iteraciones SHA256
- [x] Salt aleatorio de 256 bits
- [x] Protección contra timing attacks
- [x] Validación de contraseña fuerte (8+ chars, mayús, minús, números, especiales)
- [x] Contraseña en texto plano ? Hash + Salt en BD
- [x] Compilación exitosa
- [ ] Tests unitarios para PasswordHasher
- [ ] API sobre HTTPS en producción
- [ ] Rate limiting en login (futuro)
- [ ] 2FA (Two-Factor Authentication)

## ?? Próximos Pasos

1. **Implementar Login**
   - Endpoint POST `/api/auth/login`
   - Verificar contraseña con `PasswordHasher.VerifyPassword()`
   - Generar JWT token

2. **Migración BD**
   ```sql
   -- Hacer backup primero
   BACKUP DATABASE GestionEmpresas TO DISK = 'backup.bak'
   
   -- Ejecutar scripts SQL
   ALTER TABLE Empresas ADD ContrasenaHash VARCHAR(MAX);
   ALTER TABLE Empresas ADD ContrasenaSalt VARCHAR(MAX);
   ```

3. **Tests Unitarios**
   ```csharp
   [Fact]
   public void HashPassword_GeneratesDifferentHashForSamePassword()
   {
       var (hash1, salt1) = PasswordHasher.HashPassword("Test123!");
       var (hash2, salt2) = PasswordHasher.HashPassword("Test123!");
       
       // Deben ser diferentes (salts diferentes)
       Assert.NotEqual(hash1, hash2);
       Assert.NotEqual(salt1, salt2);
   }
   ```

4. **HTTPS Obligatorio**
   - Configurar SSL/TLS en producción
   - Redirigir HTTP a HTTPS

## ?? Especificaciones PBKDF2

| Parámetro | Valor | Estándar |
|-----------|-------|----------|
| Algoritmo | SHA256 | ? Recomendado |
| Iteraciones | 100,000 | ? NIST 2023 |
| Salt Size | 256 bits (32 bytes) | ? Mínimo recomendado |
| Hash Size | 256 bits (32 bytes) | ? SHA256 output |
| Encoding | Base64 | ? Almacenamiento |

## ? Estado Final

```
? PasswordHasher: IMPLEMENTADO
? Validación fuerte: ACTIVA
? CreateEmpresaCommand: ACTUALIZADO
? CreateEmpresaCommandHandler: HASHEA CONTRASEÑA
? Compilación: EXITOSA (0 errores)
? DTOs: ACTUALIZADOS
? Endpoint: FUNCIONANDO
```

---

**Seguridad:** ? Nivel Profesional (PBKDF2 + 100k iteraciones)
**Compilación:** ? Exitosa
**Listo para:** ? Producción (con HTTPS)
