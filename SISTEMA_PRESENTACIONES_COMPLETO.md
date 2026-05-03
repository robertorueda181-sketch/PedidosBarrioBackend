# 🎯 Sistema de Presentaciones y Opciones - Guía Completa

## 📋 Estructura Implementada

```
Producto
├── PrecioPrincipal (decimal)
└── Presentaciones[] 
    ├── NombrePresentacion (Talla, Color, Tamaño)
    └── PresentacionOpciones[]
        ├── Valor (S, M, L o Rojo, Verde, Azul)
        ├── Precio (opcional, específico de la opción)
        ├── Imagen (URL específica de la opción)
        ├── Stock (stock específico de la opción)
        └── Descripcion
```

---

## 🔗 Nuevos Endpoints

### 1️⃣ **Crear Presentación con Opciones**
**POST** `/api/Presentaciones/crear`

**Body:**
```json
{
  "descripcion": "Talla",
  "productoID": 1,
  "opciones": [
    {
      "valor": "S",
      "precio": 25.99,
      "imagen": "https://cdn.ejemplo.com/s.jpg",
      "stock": 50,
      "descripcion": "Talla Pequeña"
    },
    {
      "valor": "M",
      "precio": 29.99,
      "imagen": "https://cdn.ejemplo.com/m.jpg",
      "stock": 100,
      "descripcion": "Talla Mediana"
    },
    {
      "valor": "L",
      "precio": 33.99,
      "imagen": "https://cdn.ejemplo.com/l.jpg",
      "stock": 75,
      "descripcion": "Talla Grande"
    }
  ]
}
```

**Respuesta (201 Created):**
```json
{
  "presentacionID": 5,
  "descripcion": "Talla",
  "productoID": 1,
  "activa": true,
  "opciones": [
    {
      "presentacionOpcionID": 1,
      "valor": "S",
      "presentacionID": 5,
      "precio": 25.99,
      "imagen": "https://cdn.ejemplo.com/s.jpg",
      "descripcion": "Talla Pequeña",
      "activa": true,
      "stock": 50
    },
    // ... más opciones
  ]
}
```

---

### 2️⃣ **Descargar Plantilla Excel**
**GET** `/api/Presentaciones/descargar-plantilla`

**Respuesta:** Archivo Excel con estructura pre-configurada

**Contenido de la plantilla:**
| ProductoID* | NombrePresentacion* | ValorOpcion* | PrecioOpcion | ImagenOpcion | StockOpcion | DescripcionOpcion |
|---|---|---|---|---|---|---|
| 1 | Talla | S | 25.99 | https://... | 50 | Talla Pequeña |
| 1 | Talla | M | 29.99 | https://... | 100 | Talla Mediana |
| 1 | Color | Rojo | | https://... | 75 | Color Rojo |

*Los campos marcados con * son obligatorios

---

### 3️⃣ **Importar Presentaciones Masivamente (Excel)**
**POST** `/api/Presentaciones/importar-excel`

**Content-Type:** `multipart/form-data`

**Parámetro:** `archivo` (file)

**Respuesta (200 OK):**
```json
{
  "exitosos": 3,
  "errores": [],
  "presentaciones": [
    {
      "presentacionID": 5,
      "descripcion": "Talla",
      "productoID": 1,
      "activa": true,
      "opciones": [
        {
          "presentacionOpcionID": 1,
          "valor": "S",
          "precio": 25.99,
          "imagen": "https://cdn.ejemplo.com/s.jpg",
          "stock": 50
        },
        // ... más opciones
      ]
    },
    // ... más presentaciones
  ]
}
```

---

## 💾 Tablas de Base de Datos Nuevas

### PresentacionOpcion
```sql
CREATE TABLE PresentacionOpcion (
    PresentacionOpcionID INT PRIMARY KEY IDENTITY(1,1),
    Valor NVARCHAR(100) NOT NULL,
    PresentacionID INT NOT NULL,
    Precio DECIMAL(12,2),
    Imagen NVARCHAR(500),
    Descripcion NVARCHAR(255),
    Activa BIT DEFAULT 1,
    Stock INT,
    FOREIGN KEY (PresentacionID) REFERENCES Presentacion(PresentacionID)
);
```

### Cambios a Presentacion
```sql
ALTER TABLE Presentacion
ADD Activa BIT DEFAULT 1;
```

### Cambios a Producto
```sql
ALTER TABLE Producto
ADD PrecioPrincipal DECIMAL(12,2);
```

---

## 🎯 Casos de Uso

### Caso 1: Crear Presentación "Talla" con 3 opciones

```bash
curl -X POST "https://api.ejemplo.com/api/Presentaciones/crear" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "descripcion": "Talla",
    "productoID": 1,
    "opciones": [
      {
        "valor": "S",
        "precio": 25.99,
        "imagen": "https://cdn.ejemplo.com/talla-s.jpg",
        "stock": 50
      },
      {
        "valor": "M",
        "precio": 29.99,
        "imagen": "https://cdn.ejemplo.com/talla-m.jpg",
        "stock": 100
      },
      {
        "valor": "L",
        "precio": 33.99,
        "imagen": "https://cdn.ejemplo.com/talla-l.jpg",
        "stock": 75
      }
    ]
  }'
```

### Caso 2: Crear Presentación "Color" con 3 opciones

```bash
curl -X POST "https://api.ejemplo.com/api/Presentaciones/crear" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "descripcion": "Color",
    "productoID": 1,
    "opciones": [
      {
        "valor": "Rojo",
        "imagen": "https://cdn.ejemplo.com/color-rojo.jpg",
        "stock": 60
      },
      {
        "valor": "Verde",
        "imagen": "https://cdn.ejemplo.com/color-verde.jpg",
        "stock": 40
      },
      {
        "valor": "Azul",
        "imagen": "https://cdn.ejemplo.com/color-azul.jpg",
        "stock": 80
      }
    ]
  }'
```

### Caso 3: Importar masivamente desde Excel

```bash
# Primero descargar la plantilla
curl -X GET "https://api.ejemplo.com/api/Presentaciones/descargar-plantilla" \
  -H "Authorization: Bearer {token}" \
  -o Plantilla_Presentaciones.xlsx

# Llenar la plantilla con datos

# Luego importar
curl -X POST "https://api.ejemplo.com/api/Presentaciones/importar-excel" \
  -H "Authorization: Bearer {token}" \
  -F "archivo=@Plantilla_Presentaciones.xlsx"
```

---

## 📁 Archivos Creados/Modificados

### Nuevas Entidades
- ✅ `PedidosBarrio.Domain/Entities/PresentacionOpcion.cs`

### Nuevos DTOs
- ✅ `PedidosBarrio.Application/DTOs/PresentacionOpcionDto.cs`
- ✅ `PedidosBarrio.Application/DTOs/PresentacionDetalleDto.cs`
- ✅ `PedidosBarrio.Application/DTOs/CreatePresentacionOpcionDto.cs`
- ✅ `PedidosBarrio.Application/DTOs/CreatePresentacionDto.cs`
- ✅ `PedidosBarrio.Application/DTOs/PresentacionExcelRowDto.cs`

### Nuevos Repositories
- ✅ `PedidosBarrio.Infrastructure/Data/Repositories/PresentacionOpcionRepository.cs`
- ✅ `PedidosBarrio.Domain/Repositories/IPresentacionOpcionRepository.cs`

### Nuevos Services
- ✅ `PedidosBarrio.Application/Services/PresentacionExcelService.cs`

### Nuevos Commands
- ✅ `PedidosBarrio.Application/Commands/CreatePresentacion/CreatePresentacionCommand.cs`
- ✅ `PedidosBarrio.Application/Commands/CreatePresentacion/CreatePresentacionCommandHandler.cs`

### Nuevos Endpoints
- ✅ `PedidosBarrio/EndPoint/PresentacionEndpoint.cs`

### Archivos Modificados
- ✅ `PedidosBarrio.Domain/Entities/Presentacion.cs` - Agregada relación con PresentacionOpcion
- ✅ `PedidosBarrio.Domain/Entities/Producto.cs` - Agregado PrecioPrincipal
- ✅ `PedidosBarrio.Infrastructure/Data/Contexts/PedidosBarrioDbContext.cs` - DbSet PresentacionOpciones
- ✅ `PedidosBarrio.Infrastructure/IoC/DependencyInjection.cs` - Registrados nuevos servicios y repos
- ✅ `PedidosBarrio/Program.cs` - Mapeado nuevo endpoint
- ✅ `PedidosBarrio.Application/PedidosBarrio.Application.csproj` - Agregado ClosedXML

---

## 🔄 Flujo de Uso Completo

### Opción 1: Crear Individual
1. Hacer POST a `/api/Presentaciones/crear` con detalles de la presentación y opciones

### Opción 2: Importar Masivamente
1. Descargar plantilla: GET `/api/Presentaciones/descargar-plantilla`
2. Llenar datos en Excel
3. Importar: POST `/api/Presentaciones/importar-excel`

---

## ✅ Validaciones Incluidas

- ✅ ProductoID debe existir
- ✅ NombrePresentacion no puede estar vacío
- ✅ ValorOpcion no puede estar vacío
- ✅ PrecioOpcion es opcional (si no se especifica, puede usar el precio principal del producto)
- ✅ Imagen es opcional
- ✅ Stock es opcional
- ✅ Saltar filas incompletas automáticamente

---

## 🚀 Próximos Pasos

1. **Ejecutar migraciones de BD** para crear la tabla PresentacionOpcion
2. **Probar endpoints** en Swagger
3. **Integrar en frontend** para mostrar presentaciones y opciones

---

## 📊 Ejemplo Completo: Producto con Presentaciones Múltiples

**Producto:** Polera (ProductoID: 1)
- **PrecioPrincipal:** 35.00

**Presentación 1: Talla**
- Opción S: 25.99 (stock: 50)
- Opción M: 29.99 (stock: 100)
- Opción L: 33.99 (stock: 75)

**Presentación 2: Color**
- Opción Rojo: 35.00 (stock: 60)
- Opción Verde: 35.00 (stock: 40)
- Opción Azul: 35.00 (stock: 80)

**En Excel sería:**

| ProductoID | NombrePresentacion | ValorOpcion | PrecioOpcion | Stock |
|---|---|---|---|---|
| 1 | Talla | S | 25.99 | 50 |
| 1 | Talla | M | 29.99 | 100 |
| 1 | Talla | L | 33.99 | 75 |
| 1 | Color | Rojo | | 60 |
| 1 | Color | Verde | | 40 |
| 1 | Color | Azul | | 80 |

✅ **¡Sistema listo para usar!**
