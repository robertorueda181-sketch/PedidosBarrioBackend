# 🧪 Ejemplos de Uso - cURL y Postman

## 📍 Base URL
```
https://api.ejemplo.com
```

---

## 1️⃣ DESCARGAR PLANTILLA EXCEL

### cURL
```bash
curl -X GET "https://api.ejemplo.com/api/Presentaciones/descargar-plantilla" \
  -H "Authorization: Bearer {token}" \
  -o Plantilla_Presentaciones.xlsx
```

### Postman
- **Método:** GET
- **URL:** `{{baseUrl}}/api/Presentaciones/descargar-plantilla`
- **Headers:**
  - `Authorization: Bearer {{token}}`
- **Send** y guarda el archivo

---

## 2️⃣ CREAR PRESENTACIÓN INDIVIDUAL - TALLA

### cURL
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
  }'
```

### Postman
**Body (JSON):**
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

**Respuesta esperada (201):**
```json
{
  "presentacionID": 5,
  "descripcion": "Talla",
  "productoID": 1,
  "activa": true,
  "opciones": [
    {
      "presentacionOpcionID": 101,
      "valor": "S",
      "presentacionID": 5,
      "precio": 25.99,
      "imagen": "https://cdn.ejemplo.com/s.jpg",
      "descripcion": "Talla Pequeña",
      "activa": true,
      "stock": 50
    },
    {
      "presentacionOpcionID": 102,
      "valor": "M",
      "presentacionID": 5,
      "precio": 29.99,
      "imagen": "https://cdn.ejemplo.com/m.jpg",
      "descripcion": "Talla Mediana",
      "activa": true,
      "stock": 100
    },
    {
      "presentacionOpcionID": 103,
      "valor": "L",
      "presentacionID": 5,
      "precio": 33.99,
      "imagen": "https://cdn.ejemplo.com/l.jpg",
      "descripcion": "Talla Grande",
      "activa": true,
      "stock": 75
    }
  ]
}
```

---

## 3️⃣ CREAR PRESENTACIÓN INDIVIDUAL - COLOR

### cURL
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
        "imagen": "https://cdn.ejemplo.com/rojo.jpg",
        "stock": 60
      },
      {
        "valor": "Verde",
        "imagen": "https://cdn.ejemplo.com/verde.jpg",
        "stock": 40
      },
      {
        "valor": "Azul",
        "imagen": "https://cdn.ejemplo.com/azul.jpg",
        "stock": 80
      }
    ]
  }'
```

### Postman
**Body (JSON):**
```json
{
  "descripcion": "Color",
  "productoID": 1,
  "opciones": [
    {
      "valor": "Rojo",
      "imagen": "https://cdn.ejemplo.com/rojo.jpg",
      "stock": 60
    },
    {
      "valor": "Verde",
      "imagen": "https://cdn.ejemplo.com/verde.jpg",
      "stock": 40
    },
    {
      "valor": "Azul",
      "imagen": "https://cdn.ejemplo.com/azul.jpg",
      "stock": 80
    }
  ]
}
```

---

## 4️⃣ IMPORTAR MASIVAMENTE DESDE EXCEL

### Pasos Previos:
1. Descargar plantilla (Ver ejemplo 1)
2. Llenar con datos:

| ProductoID | NombrePresentacion | ValorOpcion | PrecioOpcion | ImagenOpcion | StockOpcion | DescripcionOpcion |
|---|---|---|---|---|---|---|
| 1 | Talla | S | 25.99 | https://cdn.ejemplo.com/s.jpg | 50 | Talla Pequeña |
| 1 | Talla | M | 29.99 | https://cdn.ejemplo.com/m.jpg | 100 | Talla Mediana |
| 1 | Talla | L | 33.99 | https://cdn.ejemplo.com/l.jpg | 75 | Talla Grande |
| 1 | Color | Rojo | | https://cdn.ejemplo.com/rojo.jpg | 60 | |
| 1 | Color | Verde | | https://cdn.ejemplo.com/verde.jpg | 40 | |
| 1 | Color | Azul | | https://cdn.ejemplo.com/azul.jpg | 80 | |
| 2 | Tamaño | Pequeño | 15.99 | https://cdn.ejemplo.com/pequeño.jpg | 100 | |
| 2 | Tamaño | Mediano | 19.99 | https://cdn.ejemplo.com/mediano.jpg | 150 | |
| 2 | Tamaño | Grande | 24.99 | https://cdn.ejemplo.com/grande.jpg | 120 | |

3. Guardar como `.xlsx`

### cURL
```bash
curl -X POST "https://api.ejemplo.com/api/Presentaciones/importar-excel" \
  -H "Authorization: Bearer {token}" \
  -F "archivo=@Plantilla_Presentaciones.xlsx"
```

### Postman
- **Método:** POST
- **URL:** `{{baseUrl}}/api/Presentaciones/importar-excel`
- **Headers:**
  - `Authorization: Bearer {{token}}`
- **Body → form-data:**
  - Key: `archivo` (type: File)
  - Value: Seleccionar archivo Excel

**Respuesta esperada (200):**
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
          "presentacionOpcionID": 101,
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
    },
    {
      "presentacionID": 6,
      "descripcion": "Color",
      "productoID": 1,
      "activa": true,
      "opciones": [
        {
          "presentacionOpcionID": 104,
          "valor": "Rojo",
          "presentacionID": 6,
          "precio": null,
          "imagen": "https://cdn.ejemplo.com/rojo.jpg",
          "descripcion": null,
          "activa": true,
          "stock": 60
        },
        // ... más opciones
      ]
    },
    {
      "presentacionID": 7,
      "descripcion": "Tamaño",
      "productoID": 2,
      "activa": true,
      "opciones": [
        // ... opciones
      ]
    }
  ]
}
```

---

## ⚠️ Errores Comunes y Soluciones

### Error 400: "Archivo no válido"
**Causa:** El archivo está vacío o no es válido
**Solución:** Verificar que el archivo esté bien formado

### Error 400: "Solo se aceptan archivos .xlsx"
**Causa:** Envías un archivo con extensión diferente
**Solución:** Guardar el archivo como `.xlsx` en Excel

### Error 400: "El archivo no contiene datos válidos"
**Causa:** Las filas están vacías o los campos obligatorios no están completos
**Solución:** Verificar que ProductoID, NombrePresentacion y ValorOpcion tengan valores

### Error 201 + "errores": ["..."]
**Causa:** Algunas filas se procesaron correctamente, pero otras tuvieron errores
**Solución:** Verificar el mensaje de error detallado y corregir esas filas

---

## 🔑 Variables de Entorno (Postman)

Crear variables en Postman para facilitar pruebas:

```
{{baseUrl}} = https://api.ejemplo.com
{{token}} = tu_token_jwt_aqui
```

---

## 📊 Ejemplo Completo: Flujo Completo

### 1. Descargar plantilla
```bash
curl -X GET "https://api.ejemplo.com/api/Presentaciones/descargar-plantilla" \
  -H "Authorization: Bearer {token}" \
  -o plantilla.xlsx
```

### 2. Abrir y llenar en Excel

### 3. Importar
```bash
curl -X POST "https://api.ejemplo.com/api/Presentaciones/importar-excel" \
  -H "Authorization: Bearer {token}" \
  -F "archivo=@plantilla.xlsx"
```

### 4. Verificar resultados
- Si `exitosos > 0`, las presentaciones fueron creadas
- Si `errores` contiene elementos, revisar qué salió mal

---

## 🧪 Script de Prueba (Bash)

```bash
#!/bin/bash

TOKEN="tu_token_aqui"
BASE_URL="https://api.ejemplo.com"

echo "1️⃣ Descargando plantilla..."
curl -X GET "$BASE_URL/api/Presentaciones/descargar-plantilla" \
  -H "Authorization: Bearer $TOKEN" \
  -o plantilla.xlsx

echo "✅ Plantilla descargada"

# Aquí llenarías manualmente el archivo...

echo "2️⃣ Importando presentaciones..."
curl -X POST "$BASE_URL/api/Presentaciones/importar-excel" \
  -H "Authorization: Bearer $TOKEN" \
  -F "archivo=@plantilla.xlsx" | jq .

echo "✅ Importación completada"
```

---

## ✅ Checklist de Prueba

- [ ] Descargar plantilla sin errores
- [ ] Crear presentación individual exitosamente
- [ ] Importar Excel con múltiples presentaciones
- [ ] Verificar que las opciones se crean correctamente
- [ ] Verificar que el stock se asigna correctamente
- [ ] Verificar que los precios se asignan correctamente
- [ ] Verificar que las imágenes se guardan correctamente
- [ ] Probar con datos incompletos (debe saltar filas)

---

**¡Sistema de presentaciones listo para usar! 🎉**
