-- ============================================
-- SCRIPT DE MIGRACION PARA PostgreSQL
-- Tabla de PageViews para Analytics
-- NOTA: Este script está diseñado para PostgreSQL
-- Para ejecutar manualmente desde pgAdmin o psql
-- ============================================

-- Tabla para almacenar las visitas de páginas
CREATE TABLE IF NOT EXISTS "PageViews" (
    "PageViewID" SERIAL PRIMARY KEY,
    "EmpresaID" UUID NOT NULL,
    "Url" VARCHAR(1000) NOT NULL,
    "Fecha" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UserAgent" VARCHAR(500),
    "IpAddress" VARCHAR(45),
    "Referrer" VARCHAR(1000),
    "Processed" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ProcessedAt" TIMESTAMP,
    CONSTRAINT "fk_pageviews_empresaid" FOREIGN KEY ("EmpresaID") REFERENCES "Empresas" ("ID") ON DELETE CASCADE
);

-- Índices para optimizar queries
CREATE INDEX IF NOT EXISTS "idx_pageviews_empresaid" ON "PageViews"("EmpresaID");
CREATE INDEX IF NOT EXISTS "idx_pageviews_fecha" ON "PageViews"("Fecha" DESC);
CREATE INDEX IF NOT EXISTS "idx_pageviews_processed" ON "PageViews"("Processed");
CREATE INDEX IF NOT EXISTS "idx_pageviews_empresaid_fecha" ON "PageViews"("EmpresaID", "Fecha" DESC);
CREATE INDEX IF NOT EXISTS "idx_pageviews_ipaddress" ON "PageViews"("IpAddress");

-- Tabla para almacenar estadísticas agregadas (opcional, para mejorar performance)
CREATE TABLE IF NOT EXISTS "PageViewStatistics" (
    "StatisticID" SERIAL PRIMARY KEY,
    "EmpresaID" UUID NOT NULL,
    "Url" VARCHAR(1000) NOT NULL,
    "Fecha" DATE NOT NULL,
    "ViewCount" INT NOT NULL DEFAULT 0,
    "UniqueIpCount" INT NOT NULL DEFAULT 0,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE("EmpresaID", "Url", "Fecha"),
    CONSTRAINT "fk_pageviewstats_empresaid" FOREIGN KEY ("EmpresaID") REFERENCES "Empresas" ("ID") ON DELETE CASCADE
);

-- Índices para estadísticas
CREATE INDEX IF NOT EXISTS "idx_pageviewstatistics_empresaid" ON "PageViewStatistics"("EmpresaID");
CREATE INDEX IF NOT EXISTS "idx_pageviewstatistics_fecha" ON "PageViewStatistics"("Fecha" DESC);
CREATE INDEX IF NOT EXISTS "idx_pageviewstatistics_empresaid_fecha" ON "PageViewStatistics"("EmpresaID", "Fecha" DESC);

-- Comentarios
COMMENT ON TABLE "PageViews" IS 'Tabla para almacenar todas las visitas a páginas de negocios';
COMMENT ON TABLE "PageViewStatistics" IS 'Tabla para almacenar estadísticas agregadas de visitas por día';
