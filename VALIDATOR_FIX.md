# ? SOLUCIÓN - Error de Validadores en MediatR

## ?? Problema Original

```
System.AggregateException: 'Some services are not able to be constructed 
(Error while validating the service descriptor 'ServiceType: MediatR.IRequestHandler`2[...
Unable to resolve service for type 'FluentValidation.IValidator`1[CreateEmpresaDto]'
```

## ? Solución Implementada

Se han creado **6 validadores** para las nuevas DTOs:

### 1?? **CreateEmpresaDtoValidator**
```
? Nombre: 255 caracteres máximo
? Email: Validación de formato
? Contraseña: 6+ caracteres
? Teléfono: Formato válido
```

### 2?? **CreateSuscripcionDtoValidator**
```
? EmpresaID: Mayor a 0
? Monto: Mayor a 0, máximo 999999.99
```

### 3?? **CreateProductoDtoValidator**
```
? EmpresaID: Mayor a 0
? Nombre: 255 caracteres máximo
? Descripción: 1000 caracteres máximo
```

### 4?? **CreateImagenDtoValidator**
```
? ProductoID: Mayor a 0
? URLImagen: URL válida, 500 caracteres máximo
? Descripción: 255 caracteres máximo
```

### 5?? **CreateInmuebleDtoValidator**
```
? EmpresaID: Mayor a 0
? TiposID: Mayor a 0
? Precio: Mayor a 0, máximo 9999999.99
? Medidas: Requeridas, 100 caracteres máximo
? Ubicación: Requerida, 500 caracteres máximo
? Dormitorios: 0-50
? Baños: 0-50
? Descripción: 1000 caracteres máximo
```

### 6?? **CreateNegocioDtoValidator**
```
? EmpresaID: Mayor a 0
? TiposID: Mayor a 0
? URLNegocio: URL válida, requerida, 500 caracteres máximo
? URLOpcional: URL válida (opcional), 500 caracteres máximo
? Descripción: 1000 caracteres máximo
```

## ?? Cambios Realizados

### 1. Creados 6 Validadores
```
PedidosBarrio.Application/Validator/
??? CreateEmpresaDtoValidator.cs ?
??? CreateSuscripcionDtoValidator.cs ?
??? CreateProductoDtoValidator.cs ?
??? CreateImagenDtoValidator.cs ?
??? CreateInmuebleDtoValidator.cs ?
??? CreateNegocioDtoValidator.cs ?
```

### 2. Actualizado DependencyInjection.cs
Se agregaron registros explícitos en el IoC Container:

```csharp
// FluentValidation - Registrar validadores específicos
services.AddScoped<IValidator<CreateEmpresaDto>, CreateEmpresaDtoValidator>();
services.AddScoped<IValidator<CreateSuscripcionDto>, CreateSuscripcionDtoValidator>();
services.AddScoped<IValidator<CreateProductoDto>, CreateProductoDtoValidator>();
services.AddScoped<IValidator<CreateImagenDto>, CreateImagenDtoValidator>();
services.AddScoped<IValidator<CreateInmuebleDto>, CreateInmuebleDtoValidator>();
services.AddScoped<IValidator<CreateNegocioDto>, CreateNegocioDtoValidator>();
```

## ?? Estado Actual

```
? Build: EXITOSO (0 errores)
? Validadores: REGISTRADOS
? MediatR: CONFIGURADO
? Handlers: LISTOS
```

## ?? Próximos Pasos

1. **Ejecutar la aplicación:**
```bash
dotnet run
```

2. **Acceder a Swagger:**
```
https://localhost:7045/swagger
```

3. **Probar los endpoints:**
- POST `/api/Empresas` - Crear empresa con validación
- POST `/api/Suscripciones` - Crear suscripción con validación
- POST `/api/Productos` - Crear producto con validación
- POST `/api/Imagenes` - Crear imagen con validación
- POST `/api/Inmuebles` - Crear inmueble con validación
- POST `/api/Negocios` - Crear negocio con validación

## ? Ejemplo de Respuesta Validada

**Request:** Crear Empresa con email inválido
```json
POST /api/Empresas
{
  "nombre": "Mi Empresa",
  "email": "email-invalido",  // ? NO ES UN EMAIL VÁLIDO
  "contrasena": "pass123",
  "telefono": "+34 123456789"
}
```

**Response (400 Bad Request):**
```json
{
  "errors": [
    {
      "property": "email",
      "message": "El email debe ser válido."
    }
  ]
}
```

## ?? Validaciones Activas

| DTO | Validaciones | Estado |
|-----|-------------|--------|
| CreateEmpresaDto | 4 | ? Activo |
| CreateSuscripcionDto | 2 | ? Activo |
| CreateProductoDto | 3 | ? Activo |
| CreateImagenDto | 3 | ? Activo |
| CreateInmuebleDto | 8 | ? Activo |
| CreateNegocioDto | 5 | ? Activo |

**Total de validaciones: 25 reglas**

## ? Verificación

```csharp
// ? Esto ahora funciona correctamente
var empresaHandler = serviceProvider.GetService<
    IRequestHandler<CreateEmpresaCommand, EmpresaDto>
>();
// IValidator<CreateEmpresaDto> se inyecta automáticamente
```

---

**Estado:** ? **RESUELTO**
**Versión:** 1.1  
**Cambios:** 7 archivos nuevos, 1 actualizado
