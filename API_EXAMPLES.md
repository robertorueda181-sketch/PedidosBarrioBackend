# ?? EJEMPLOS DE USO DE LA API

## Base URL
```
https://localhost:7045/api
```

## ?? EMPRESAS

### 1. Crear Empresa
```http
POST /api/Empresas
Content-Type: application/json

{
  "nombre": "Tech Solutions SA",
  "email": "info@techsolutions.com",
  "contrasena": "hash_seguro_aqui",
  "telefono": "+34 912345678"
}
```

**Respuesta (201):**
```json
{
  "empresaID": 5,
  "nombre": "Tech Solutions SA",
  "email": "info@techsolutions.com",
  "telefono": "+34 912345678",
  "fechaRegistro": "2024-01-16T10:30:00Z",
  "activa": true
}
```

### 2. Obtener todas las Empresas
```http
GET /api/Empresas
```

**Respuesta (200):**
```json
[
  {
    "empresaID": 1,
    "nombre": "Empresa Test",
    "email": "test@empresa.com",
    "telefono": "+34 123456789",
    "fechaRegistro": "2024-01-15T10:00:00Z",
    "activa": true
  }
]
```

### 3. Obtener Empresa por ID
```http
GET /api/Empresas/1
```

**Respuesta (200):**
```json
{
  "empresaID": 1,
  "nombre": "Empresa Test",
  "email": "test@empresa.com",
  "telefono": "+34 123456789",
  "fechaRegistro": "2024-01-15T10:00:00Z",
  "activa": true
}
```

### 4. Actualizar Empresa
```http
PUT /api/Empresas/1
Content-Type: application/json

{
  "empresaID": 1,
  "nombre": "Empresa Test Actualizada",
  "email": "nuevo@empresa.com",
  "telefono": "+34 987654321",
  "activa": true
}
```

**Respuesta (204): No Content**

### 5. Eliminar Empresa (Soft Delete)
```http
DELETE /api/Empresas/1
```

**Respuesta (204): No Content**

---

## ?? SUSCRIPCIONES

### 1. Crear Suscripción
```http
POST /api/Suscripciones
Content-Type: application/json

{
  "empresaID": 1,
  "monto": 99.99,
  "fechaFin": "2024-02-16T23:59:59Z"
}
```

**Respuesta (201):**
```json
{
  "suscripcionID": 3,
  "empresaID": 1,
  "fechaInicio": "2024-01-16T10:30:00Z",
  "fechaFin": "2024-02-16T23:59:59Z",
  "activa": true,
  "monto": 99.99
}
```

### 2. Obtener Suscripciones de una Empresa
```http
GET /api/Suscripciones/empresa/1
```

**Respuesta (200):**
```json
[
  {
    "suscripcionID": 1,
    "empresaID": 1,
    "fechaInicio": "2024-01-15T10:00:00Z",
    "fechaFin": "2024-02-15T23:59:59Z",
    "activa": true,
    "monto": 99.99
  }
]
```

### 3. Actualizar Suscripción
```http
PUT /api/Suscripciones/1
Content-Type: application/json

{
  "suscripcionID": 1,
  "empresaID": 1,
  "monto": 149.99,
  "fechaFin": "2024-03-15T23:59:59Z",
  "activa": true
}
```

**Respuesta (204): No Content**

### 4. Eliminar Suscripción (Soft Delete)
```http
DELETE /api/Suscripciones/1
```

**Respuesta (204): No Content**

---

## ?? PRODUCTOS

### 1. Crear Producto
```http
POST /api/Productos
Content-Type: application/json

{
  "empresaID": 1,
  "nombre": "Laptop HP Pavilion",
  "descripcion": "Laptop de alta gama para profesionales"
}
```

**Respuesta (201):**
```json
{
  "productoID": 2,
  "empresaID": 1,
  "nombre": "Laptop HP Pavilion",
  "descripcion": "Laptop de alta gama para profesionales",
  "fechaCreacion": "2024-01-16T10:35:00Z"
}
```

### 2. Obtener Productos de una Empresa
```http
GET /api/Productos/empresa/1
```

**Respuesta (200):**
```json
[
  {
    "productoID": 1,
    "empresaID": 1,
    "nombre": "Producto Test",
    "descripcion": "Descripción del producto de prueba",
    "fechaCreacion": "2024-01-15T10:05:00Z"
  }
]
```

### 3. Actualizar Producto
```http
PUT /api/Productos/1
Content-Type: application/json

{
  "productoID": 1,
  "empresaID": 1,
  "nombre": "Producto Test Actualizado",
  "descripcion": "Nueva descripción"
}
```

**Respuesta (204): No Content**

### 4. Eliminar Producto
```http
DELETE /api/Productos/1
```

**Respuesta (204): No Content**

---

## ??? IMÁGENES

### 1. Crear Imagen
```http
POST /api/Imagenes
Content-Type: application/json

{
  "productoID": 2,
  "urlImagen": "https://cdn.example.com/laptop-hp-1.jpg",
  "descripcion": "Vista frontal del producto"
}
```

**Respuesta (201):**
```json
{
  "imagenID": 3,
  "productoID": 2,
  "urlImagen": "https://cdn.example.com/laptop-hp-1.jpg",
  "descripcion": "Vista frontal del producto"
}
```

### 2. Obtener Imágenes de un Producto
```http
GET /api/Imagenes/producto/2
```

**Respuesta (200):**
```json
[
  {
    "imagenID": 3,
    "productoID": 2,
    "urlImagen": "https://cdn.example.com/laptop-hp-1.jpg",
    "descripcion": "Vista frontal del producto"
  }
]
```

### 3. Actualizar Imagen
```http
PUT /api/Imagenes/3
Content-Type: application/json

{
  "imagenID": 3,
  "productoID": 2,
  "urlImagen": "https://cdn.example.com/laptop-hp-updated.jpg",
  "descripcion": "Vista actualizada del producto"
}
```

**Respuesta (204): No Content**

---

## ??? TIPOS (Solo Lectura)

### Obtener Todos los Tipos
```http
GET /api/Tipos
```

**Respuesta (200):**
```json
[
  {
    "tiposID": 1,
    "tipoNombre": "Apartamento",
    "categoria": "INM"
  },
  {
    "tiposID": 2,
    "tipoNombre": "Casa",
    "categoria": "INM"
  },
  {
    "tiposID": 3,
    "tipoNombre": "Local Comercial",
    "categoria": "NEG"
  }
]
```

---

## ?? INMUEBLES

### 1. Crear Inmueble
```http
POST /api/Inmuebles
Content-Type: application/json

{
  "empresaID": 1,
  "tiposID": 2,
  "precio": 350000.00,
  "medidas": "150m²",
  "ubicacion": "Calle Principal 123, Madrid",
  "dormitorios": 3,
  "banos": 2,
  "descripcion": "Casa moderna con piscina"
}
```

**Respuesta (201):**
```json
{
  "inmuebleID": 1,
  "empresaID": 1,
  "tiposID": 2,
  "precio": 350000.00,
  "medidas": "150m²",
  "ubicacion": "Calle Principal 123, Madrid",
  "dormitorios": 3,
  "banos": 2,
  "descripcion": "Casa moderna con piscina",
  "fechaRegistro": "2024-01-16T10:40:00Z"
}
```

### 2. Obtener Inmuebles de una Empresa
```http
GET /api/Inmuebles/empresa/1
```

**Respuesta (200):**
```json
[
  {
    "inmuebleID": 1,
    "empresaID": 1,
    "tiposID": 2,
    "precio": 350000.00,
    "medidas": "150m²",
    "ubicacion": "Calle Principal 123, Madrid",
    "dormitorios": 3,
    "banos": 2,
    "descripcion": "Casa moderna con piscina",
    "fechaRegistro": "2024-01-16T10:40:00Z"
  }
]
```

### 3. Actualizar Inmueble
```http
PUT /api/Inmuebles/1
Content-Type: application/json

{
  "inmuebleID": 1,
  "empresaID": 1,
  "tiposID": 2,
  "precio": 375000.00,
  "medidas": "150m²",
  "ubicacion": "Calle Principal 123, Madrid",
  "dormitorios": 3,
  "banos": 2,
  "descripcion": "Casa moderna con piscina - Precio actualizado"
}
```

**Respuesta (204): No Content**

---

## ?? NEGOCIOS

### 1. Crear Negocio
```http
POST /api/Negocios
Content-Type: application/json

{
  "empresaID": 1,
  "tiposID": 3,
  "urlNegocio": "https://www.ejemplo-negocio.com",
  "descripcion": "Tienda online de venta de electrónica",
  "urlOpcional": "https://www.instagram.com/ejemplo-negocio"
}
```

**Respuesta (201):**
```json
{
  "negocioID": 1,
  "empresaID": 1,
  "tiposID": 3,
  "urlNegocio": "https://www.ejemplo-negocio.com",
  "urlOpcional": "https://www.instagram.com/ejemplo-negocio",
  "descripcion": "Tienda online de venta de electrónica",
  "fechaRegistro": "2024-01-16T10:45:00Z"
}
```

### 2. Obtener Negocios de una Empresa
```http
GET /api/Negocios/empresa/1
```

**Respuesta (200):**
```json
[
  {
    "negocioID": 1,
    "empresaID": 1,
    "tiposID": 3,
    "urlNegocio": "https://www.ejemplo-negocio.com",
    "urlOpcional": "https://www.instagram.com/ejemplo-negocio",
    "descripcion": "Tienda online de venta de electrónica",
    "fechaRegistro": "2024-01-16T10:45:00Z"
  }
]
```

### 3. Actualizar Negocio
```http
PUT /api/Negocios/1
Content-Type: application/json

{
  "negocioID": 1,
  "empresaID": 1,
  "tiposID": 3,
  "urlNegocio": "https://www.ejemplo-negocio.com",
  "urlOpcional": "https://www.instagram.com/ejemplo-negocio",
  "descripcion": "Tienda online de venta de electrónica - Actualizada"
}
```

**Respuesta (204): No Content**

---

## ?? Códigos de Respuesta HTTP

| Código | Significado |
|--------|-----------|
| 200 | OK - Operación exitosa |
| 201 | Created - Recurso creado exitosamente |
| 204 | No Content - Operación exitosa sin contenido |
| 400 | Bad Request - Datos inválidos |
| 404 | Not Found - Recurso no encontrado |
| 500 | Internal Server Error - Error del servidor |

---

## ?? Notas Importantes

1. **Soft Delete**: Empresa y Suscripción solo marcan como inactiva (`Activa = 0`)
2. **Hard Delete**: Producto, Imagen, Inmueble y Negocio se eliminan completamente
3. **Fecha**: Se envía en formato ISO 8601 (UTC)
4. **ID**: Siempre es numérico (INT)

---

¡Listo! Ahora puedes usar cualquiera de estos ejemplos para probar la API.
