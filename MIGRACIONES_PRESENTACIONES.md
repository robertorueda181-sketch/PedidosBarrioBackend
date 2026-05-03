# 🗄️ Migraciones de Base de Datos - Sistema de Presentaciones

## 📋 Cambios Necesarios en BD

Después de actualizar el código, necesitas ejecutar estas migraciones en tu PostgreSQL.

### 1️⃣ Crear tabla PresentacionOpcion

```sql
CREATE TABLE IF NOT EXISTS "PresentacionOpcion" (
    "PresentacionOpcionID" SERIAL PRIMARY KEY,
    "Valor" VARCHAR(100) NOT NULL,
    "PresentacionID" INTEGER NOT NULL,
    "Precio" NUMERIC(12, 2),
    "Imagen" VARCHAR(500),
    "Descripcion" VARCHAR(255),
    "Activa" BOOLEAN NOT NULL DEFAULT TRUE,
    "Stock" INTEGER,
    CONSTRAINT "FK_PresentacionOpcion_Presentacion" FOREIGN KEY ("PresentacionID") 
        REFERENCES "Presentacion"("PresentacionID") ON DELETE CASCADE
);

-- Crear índice para búsquedas rápidas
CREATE INDEX "IX_PresentacionOpcion_PresentacionID" 
    ON "PresentacionOpcion"("PresentacionID");
```

### 2️⃣ Agregar columna Activa a Presentacion

```sql
ALTER TABLE "Presentacion"
ADD COLUMN "Activa" BOOLEAN NOT NULL DEFAULT TRUE;
```

### 3️⃣ Agregar columna PrecioPrincipal a Producto

```sql
ALTER TABLE "Producto"
ADD COLUMN "PrecioPrincipal" NUMERIC(12, 2);
```

---

## 🔄 Opción A: Usar EF Core Migrations (Recomendado)

### Paso 1: Crear Migración
```powershell
cd PedidosBarrio.Infrastructure
dotnet ef migrations add AddPresentacionOpcionAndPrices --project ../PedidosBarrio.Infrastructure/PedidosBarrio.Infrastructure.csproj
```

### Paso 2: Aplicar Migración
```powershell
dotnet ef database update --project ../PedidosBarrio.Infrastructure/PedidosBarrio.Infrastructure.csproj
```

### Verificar Migración Creada
```powershell
dotnet ef migrations list
```

---

## 🔄 Opción B: SQL Directo (Si prefieres)

Conecta a tu PostgreSQL y ejecuta los scripts SQL anteriores.

---

## ✅ Verificar que todo está correcto

### Verificar tablas creadas
```sql
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
  AND table_name IN ('PresentacionOpcion', 'Presentacion', 'Producto');
```

### Verificar columnas en Presentacion
```sql
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'Presentacion';
```

### Verificar columnas en Producto
```sql
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'Producto'
AND column_name IN ('PrecioPrincipal');
```

### Verificar columnas en PresentacionOpcion
```sql
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'PresentacionOpcion';
```

---

## 🧪 Probar con datos de ejemplo

```sql
-- Insertar presentación de ejemplo
INSERT INTO "Presentacion" ("Descripcion", "EmpresaID", "ProductoID", "Activa")
VALUES ('Talla', '550e8400-e29b-41d4-a716-446655440000', 1, TRUE)
RETURNING "PresentacionID";
-- Guardá el PresentacionID (ej: 1)

-- Insertar opciones de ejemplo
INSERT INTO "PresentacionOpcion" ("Valor", "PresentacionID", "Precio", "Stock", "Activa")
VALUES 
  ('S', 1, 25.99, 50, TRUE),
  ('M', 1, 29.99, 100, TRUE),
  ('L', 1, 33.99, 75, TRUE);

-- Verificar datos insertados
SELECT * FROM "PresentacionOpcion" WHERE "PresentacionID" = 1;
```

---

## 📊 Diagrama de Relaciones

```
┌─────────────┐
│   Producto  │
├─────────────┤
│ ProductoID  │◄──────────┐
│ Nombre      │           │
│ Descripción │           │ Foreign Key
│ Precio      │           │
│ PrecioPrinc.│           │
└─────────────┘           │
                          │
                    ┌─────────────────┐
                    │  Presentacion   │
                    ├─────────────────┤
                    │ PresentacionID  │◄──────────┐
                    │ Descripcion     │           │
                    │ ProductoID      │           │ Foreign Key
                    │ Activa          │           │
                    └─────────────────┘           │
                                                  │
                                    ┌──────────────────────────┐
                                    │ PresentacionOpcion       │
                                    ├──────────────────────────┤
                                    │ PresentacionOpcionID     │
                                    │ Valor                    │
                                    │ PresentacionID           │
                                    │ Precio                   │
                                    │ Imagen                   │
                                    │ Stock                    │
                                    │ Descripcion              │
                                    │ Activa                   │
                                    └──────────────────────────┘
```

---

## 🚀 Después de las Migraciones

1. **Compilar el proyecto:**
   ```powershell
   dotnet build
   ```

2. **Probar los endpoints:**
   - GET `/api/Presentaciones/descargar-plantilla`
   - POST `/api/Presentaciones/crear`
   - POST `/api/Presentaciones/importar-excel`

3. **Verificar en Swagger:**
   - La API debe mostrar los nuevos endpoints

---

## ⚠️ Troubleshooting

### Error: "Table already exists"
Si recibes este error, la tabla ya existe. Verifica con:
```sql
DROP TABLE IF EXISTS "PresentacionOpcion" CASCADE;
```

### Error: "Foreign Key constraint failed"
Asegúrate de que:
1. El ProductoID existe en la tabla Producto
2. El PresentacionID existe en la tabla Presentacion

### Error en EF Core Migrations
Si falla la migración:
```powershell
# Ver el historial
dotnet ef migrations list

# Revertir última migración (si aplica)
dotnet ef migrations remove

# Crear nuevamente
dotnet ef migrations add AddPresentacionSystem
```

---

## ✅ Checklist Final

- [ ] Migración creada
- [ ] Migraciones aplicadas a BD
- [ ] Tablas verificadas en PostgreSQL
- [ ] Endpoints disponibles en Swagger
- [ ] Código compilado sin errores
- [ ] Datos de prueba insertados
- [ ] Pruebas unitarias pasadas (si existen)
- [ ] Listo para desplegar

---

**¡Estás listo para usar el sistema de presentaciones! 🎉**
