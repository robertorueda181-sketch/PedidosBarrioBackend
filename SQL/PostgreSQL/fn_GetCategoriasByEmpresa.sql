-- Función PostgreSQL para obtener categorías por empresa con filtro de mostrar
DROP FUNCTION IF EXISTS fn_GetCategoriasByEmpresaMostrando(UUID);

CREATE OR REPLACE FUNCTION fn_GetCategoriasByEmpresaMostrando(p_empresa_id UUID)
RETURNS TABLE (
    categoria_id SMALLINT,
    empresa_id UUID,
    descripcion VARCHAR(50),
    codigo VARCHAR(10),
    activo BOOLEAN,
    mostrar BOOLEAN
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        c."CategoriaID"::SMALLINT,
        c."EmpresaID",
        c."Descripcion",
        c."Codigo",
        c."Activo",
        c."Mostrar"
    FROM "Categorias" c
    WHERE c."EmpresaID" = p_empresa_id
    AND c."Mostrar" = true
    AND c."Activo" = true
    ORDER BY c."Descripcion" ASC;
END;
$$ LANGUAGE plpgsql;

-- Función adicional para obtener todas las categorías por empresa
DROP FUNCTION IF EXISTS fn_GetCategoriasByEmpresa(UUID);

CREATE OR REPLACE FUNCTION fn_GetCategoriasByEmpresa(p_empresa_id UUID)
RETURNS TABLE (
    categoria_id SMALLINT,
    empresa_id UUID,
    descripcion VARCHAR(50),
    codigo VARCHAR(10),
    activo BOOLEAN,
    mostrar BOOLEAN
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        c."CategoriaID"::SMALLINT,
        c."EmpresaID",
        c."Descripcion",
        c."Codigo",
        c."Activo",
        c."Mostrar"
    FROM "Categorias" c
    WHERE c."EmpresaID" = p_empresa_id
    ORDER BY c."Descripcion" ASC;
END;
$$ LANGUAGE plpgsql;

-- Ejemplos de uso:
-- SELECT * FROM fn_GetCategoriasByEmpresaMostrando('550e8400-e29b-41d4-a716-446655440000'::UUID);
-- SELECT * FROM fn_GetCategoriasByEmpresa('550e8400-e29b-41d4-a716-446655440000'::UUID);
