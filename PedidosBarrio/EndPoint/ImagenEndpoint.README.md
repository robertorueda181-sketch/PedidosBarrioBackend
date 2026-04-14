# Endpoint de Optimización de Imágenes

## 📝 Descripción
Endpoint genérico para optimizar, redimensionar y convertir imágenes a formato WebP según el tipo especificado.

## 🔗 URL
```
POST /api/Imagenes/optimize
```

## 📋 Parámetros

| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| `imagen` | File | Sí | Archivo de imagen a procesar (JPG, PNG, GIF, WebP) |
| `tipo` | String | Sí | Tipo de imagen que determina las dimensiones |

## 🎨 Tipos de Imagen Soportados

| Tipo | Dimensiones | Uso Recomendado | Calidad WebP |
|------|-------------|-----------------|--------------|
| `Banner` | 1200x600 | Banners, cabeceras, sliders | 80% |
| `Producto` | 400x400 | Imágenes de productos | 75% |
| `Empresa` | 300x300 | Logos de empresas | 75% |
| `Categoria` | 500x500 | Imágenes de categorías | 75% |
| `Avatar` | 200x200 | Fotos de perfil, avatares | 75% |
| `Original` | Sin redimensionar | Conserva dimensiones originales | 85% |

## 📤 Ejemplo de Petición

### cURL
```bash
curl -X POST "https://tu-api.com/api/Imagenes/optimize" \
  -H "Content-Type: multipart/form-data" \
  -F "imagen=@/path/to/image.jpg" \
  -F "tipo=Banner"
```

### JavaScript (Fetch)
```javascript
const formData = new FormData();
formData.append('imagen', imageFile);
formData.append('tipo', 'Banner');

const response = await fetch('/api/Imagenes/optimize', {
  method: 'POST',
  body: formData
});

const result = await response.json();
console.log('URL de imagen:', result.url);
```

### C# (HttpClient)
```csharp
using var client = new HttpClient();
using var content = new MultipartFormDataContent();

var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync("image.jpg"));
fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
content.Add(fileContent, "imagen", "image.jpg");
content.Add(new StringContent("Banner"), "tipo");

var response = await client.PostAsync("/api/Imagenes/optimize", content);
var result = await response.Content.ReadFromJsonAsync<OptimizeImageResponseDto>();
```

## ✅ Respuesta Exitosa (200 OK)

```json
{
  "url": "https://tu-api.com/images/banners/12345678-90ab-cdef-1234-567890abcdef_20240115120000.webp",
  "tipoImagen": "Banner",
  "dimensiones": "1200x600",
  "formato": "webp",
  "mensaje": "Imagen optimizada y guardada exitosamente como Banner"
}
```

## ❌ Respuestas de Error

### 400 Bad Request - Sin imagen
```json
{
  "error": "No se ha proporcionado ninguna imagen."
}
```

### 400 Bad Request - Tipo inválido
```json
{
  "error": "Tipo de imagen inválido: 'InvalidType'. Tipos válidos: Banner, Producto, Empresa, Categoria, Avatar, Original"
}
```

### 400 Bad Request - Archivo muy grande
```json
{
  "error": "El archivo es demasiado grande. Tamaño máximo: 10MB"
}
```

### 500 Internal Server Error
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Error al procesar la imagen",
  "status": 500,
  "detail": "Error específico del procesamiento"
}
```

## 🔧 Características Técnicas

### Validaciones
- ✅ Tamaño máximo: 10 MB
- ✅ Formatos permitidos: JPG, JPEG, PNG, GIF, WebP
- ✅ Validación de tipo de imagen

### Procesamiento
- 🔄 Redimensionamiento según tipo
- 🗜️ Compresión WebP optimizada
- 📁 Almacenamiento organizado por tipo
- 🔐 Nombres únicos con GUID + timestamp

### Estructura de Carpetas
```
wwwroot/
└── images/
    ├── banners/
    ├── productos/
    ├── categorias/
    ├── avatars/
    └── originals/
```

## 🎯 Casos de Uso

### 1. Subir Banner de Página
```javascript
// Banner principal de la página (1200x600)
formData.append('tipo', 'Banner');
```

### 2. Subir Imagen de Producto
```javascript
// Imagen de producto (400x400)
formData.append('tipo', 'Producto');
```

### 3. Subir Logo de Empresa
```javascript
// Logo de empresa (300x300)
formData.append('tipo', 'Empresa');
```

### 4. Subir Avatar de Usuario
```javascript
// Foto de perfil (200x200)
formData.append('tipo', 'Avatar');
```

### 5. Subir Imagen Original
```javascript
// Sin redimensionar, solo convierte a WebP
formData.append('tipo', 'Original');
```

## 🔒 Seguridad
- Sin autenticación requerida (ajustar según necesidades)
- Validación de tipo de archivo
- Límite de tamaño de archivo
- Nombres únicos para evitar colisiones

## 📊 Performance
- Conversión a WebP reduce tamaño hasta 30-50%
- Redimensionamiento optimizado con ImageSharp
- Compresión con balance calidad/tamaño

## 🚀 Mejoras Futuras
- [ ] Agregar autenticación/autorización
- [ ] Implementar límite de rate limiting
- [ ] Agregar watermark opcional
- [ ] Generar múltiples tamaños (thumbnails)
- [ ] Almacenamiento en cloud (Azure Blob, AWS S3)
- [ ] CDN para servir imágenes
