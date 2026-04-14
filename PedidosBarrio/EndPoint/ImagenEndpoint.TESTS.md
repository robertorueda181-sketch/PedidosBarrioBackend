# Ejemplos de Prueba - Endpoint de Optimización de Imágenes

## Ejemplos con cURL

### 1. Optimizar Banner (1200x600)
```bash
curl -X POST "http://localhost:5000/api/Imagenes/optimize" \
  -F "imagen=@banner.jpg" \
  -F "tipo=Banner"
```

### 2. Optimizar Producto (400x400)
```bash
curl -X POST "http://localhost:5000/api/Imagenes/optimize" \
  -F "imagen=@producto.png" \
  -F "tipo=Producto"
```

### 3. Optimizar Logo de Empresa (300x300)
```bash
curl -X POST "http://localhost:5000/api/Imagenes/optimize" \
  -F "imagen=@logo.png" \
  -F "tipo=Empresa"
```

### 4. Optimizar Categoría (500x500)
```bash
curl -X POST "http://localhost:5000/api/Imagenes/optimize" \
  -F "imagen=@categoria.jpg" \
  -F "tipo=Categoria"
```

### 5. Optimizar Avatar (200x200)
```bash
curl -X POST "http://localhost:5000/api/Imagenes/optimize" \
  -F "imagen=@avatar.jpg" \
  -F "tipo=Avatar"
```

### 6. Optimizar sin Redimensionar (Original)
```bash
curl -X POST "http://localhost:5000/api/Imagenes/optimize" \
  -F "imagen=@imagen.jpg" \
  -F "tipo=Original"
```

## Ejemplos con PowerShell

### 1. Banner
```powershell
$form = @{
    imagen = Get-Item -Path "C:\imagenes\banner.jpg"
    tipo = "Banner"
}

Invoke-RestMethod -Uri "http://localhost:5000/api/Imagenes/optimize" `
    -Method Post -Form $form
```

### 2. Producto
```powershell
$form = @{
    imagen = Get-Item -Path "C:\imagenes\producto.png"
    tipo = "Producto"
}

Invoke-RestMethod -Uri "http://localhost:5000/api/Imagenes/optimize" `
    -Method Post -Form $form
```

## Ejemplos con JavaScript (React/Next.js)

### Componente de Upload
```javascript
import { useState } from 'react';

export default function ImageUploader() {
  const [uploading, setUploading] = useState(false);
  const [result, setResult] = useState(null);

  const handleUpload = async (event, tipo) => {
    const file = event.target.files[0];
    if (!file) return;

    setUploading(true);
    const formData = new FormData();
    formData.append('imagen', file);
    formData.append('tipo', tipo);

    try {
      const response = await fetch('/api/Imagenes/optimize', {
        method: 'POST',
        body: formData,
      });

      const data = await response.json();
      setResult(data);
      console.log('Imagen optimizada:', data.url);
    } catch (error) {
      console.error('Error:', error);
    } finally {
      setUploading(false);
    }
  };

  return (
    <div>
      <h2>Subir Banner</h2>
      <input
        type="file"
        accept="image/*"
        onChange={(e) => handleUpload(e, 'Banner')}
        disabled={uploading}
      />

      <h2>Subir Producto</h2>
      <input
        type="file"
        accept="image/*"
        onChange={(e) => handleUpload(e, 'Producto')}
        disabled={uploading}
      />

      {result && (
        <div>
          <h3>Resultado:</h3>
          <img src={result.url} alt="Imagen optimizada" />
          <pre>{JSON.stringify(result, null, 2)}</pre>
        </div>
      )}
    </div>
  );
}
```

## Ejemplos con Postman

### Configuración
1. Método: `POST`
2. URL: `http://localhost:5000/api/Imagenes/optimize`
3. Headers: (ninguno necesario, se configura automáticamente)
4. Body:
   - Tipo: `form-data`
   - Key 1: `imagen` (tipo: File) - Seleccionar archivo
   - Key 2: `tipo` (tipo: Text) - Valor: `Banner`, `Producto`, etc.

### Respuesta Esperada
```json
{
  "url": "http://localhost:5000/images/banners/a1b2c3d4-...-20240115120000.webp",
  "tipoImagen": "Banner",
  "dimensiones": "1200x600",
  "formato": "webp",
  "mensaje": "Imagen optimizada y guardada exitosamente como Banner"
}
```

## Pruebas Automatizadas con C# (xUnit)

```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

public class ImageOptimizeEndpointTests
{
    private readonly HttpClient _client;

    public ImageOptimizeEndpointTests()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000")
        };
    }

    [Theory]
    [InlineData("Banner", "1200x600")]
    [InlineData("Producto", "400x400")]
    [InlineData("Empresa", "300x300")]
    [InlineData("Categoria", "500x500")]
    [InlineData("Avatar", "200x200")]
    public async Task OptimizeImage_ReturnsCorrectDimensions(string tipo, string expectedDimensions)
    {
        // Arrange
        var imageBytes = await File.ReadAllBytesAsync("test-image.jpg");
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(imageBytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(fileContent, "imagen", "test.jpg");
        content.Add(new StringContent(tipo), "tipo");

        // Act
        var response = await _client.PostAsync("/api/Imagenes/optimize", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<OptimizeImageResponseDto>();
        Assert.NotNull(result);
        Assert.Equal(tipo, result.TipoImagen);
        Assert.Equal(expectedDimensions, result.Dimensiones);
        Assert.Equal("webp", result.Formato);
        Assert.NotEmpty(result.Url);
    }

    [Fact]
    public async Task OptimizeImage_WithoutImage_ReturnsBadRequest()
    {
        // Arrange
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("Banner"), "tipo");

        // Act
        var response = await _client.PostAsync("/api/Imagenes/optimize", content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task OptimizeImage_WithInvalidType_ReturnsBadRequest()
    {
        // Arrange
        var imageBytes = await File.ReadAllBytesAsync("test-image.jpg");
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(imageBytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(fileContent, "imagen", "test.jpg");
        content.Add(new StringContent("InvalidType"), "tipo");

        // Act
        var response = await _client.PostAsync("/api/Imagenes/optimize", content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
```

## Script de Prueba Masiva (Bash)

```bash
#!/bin/bash

# Array de tipos de imágenes
tipos=("Banner" "Producto" "Empresa" "Categoria" "Avatar" "Original")

# Archivo de imagen de prueba
test_image="test-image.jpg"

# URL del endpoint
url="http://localhost:5000/api/Imagenes/optimize"

echo "Iniciando pruebas de optimización de imágenes..."

for tipo in "${tipos[@]}"
do
    echo "Probando tipo: $tipo"
    response=$(curl -s -X POST "$url" \
        -F "imagen=@$test_image" \
        -F "tipo=$tipo")

    echo "Respuesta: $response"
    echo "---"
    sleep 1
done

echo "Pruebas completadas!"
```

## Validaciones de Error

### Error: Imagen muy grande
```bash
# Intentar subir archivo > 10MB
curl -X POST "http://localhost:5000/api/Imagenes/optimize" \
  -F "imagen=@imagen-grande.jpg" \
  -F "tipo=Banner"

# Respuesta esperada:
# {
#   "error": "El archivo es demasiado grande. Tamaño máximo: 10MB"
# }
```

### Error: Tipo inválido
```bash
curl -X POST "http://localhost:5000/api/Imagenes/optimize" \
  -F "imagen=@imagen.jpg" \
  -F "tipo=TipoInvalido"

# Respuesta esperada:
# {
#   "error": "Tipo de imagen inválido: 'TipoInvalido'. Tipos válidos: Banner, Producto, Empresa, Categoria, Avatar, Original"
# }
```

### Error: Sin imagen
```bash
curl -X POST "http://localhost:5000/api/Imagenes/optimize" \
  -F "tipo=Banner"

# Respuesta esperada:
# {
#   "error": "No se ha proporcionado ninguna imagen."
# }
```
