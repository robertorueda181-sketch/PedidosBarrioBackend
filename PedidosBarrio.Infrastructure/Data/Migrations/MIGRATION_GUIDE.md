# Migración EF Core - DeviceTokens Table

## Crear la Migración

```bash
cd PedidosBarrio.Infrastructure
dotnet ef migrations add AddDeviceTokensTable --project PedidosBarrio.Infrastructure --startup-project PedidosBarrio
```

## Actualizar la Base de Datos

```bash
dotnet ef database update --project PedidosBarrio.Infrastructure --startup-project PedidosBarrio
```

## SQL Generado (Referencia)

```sql
CREATE TABLE IF NOT EXISTS "DeviceTokens" (
    "DeviceTokenID" SERIAL PRIMARY KEY,
    "Token" VARCHAR(500) NOT NULL UNIQUE,
    "ClienteID" INTEGER NULL,
    "EmpresaID" UUID NULL,
    "Platform" VARCHAR(50) NULL,
    "DeviceId" VARCHAR(255) NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "RegisteredDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "LastUsedDate" TIMESTAMP NULL
);

CREATE INDEX "idx_device_tokens_active" ON "DeviceTokens"("IsActive");
CREATE INDEX "idx_device_tokens_token" ON "DeviceTokens"("Token");
CREATE INDEX "idx_device_tokens_empresa" ON "DeviceTokens"("EmpresaID");
CREATE INDEX "idx_device_tokens_cliente" ON "DeviceTokens"("ClienteID");
```

## Verificar la Tabla

```sql
-- PostgreSQL
SELECT * FROM "DeviceTokens" LIMIT 10;

-- Ver estructura
\d "DeviceTokens"

-- Ver índices
SELECT indexname FROM pg_indexes WHERE tablename = 'DeviceTokens';
```

## Rollback (si es necesario)

```bash
dotnet ef migrations remove --project PedidosBarrio.Infrastructure --startup-project PedidosBarrio
```
