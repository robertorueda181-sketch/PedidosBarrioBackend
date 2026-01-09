-- Función PostgreSQL para obtener productos por ID de empresa (UUID)
-- Esta función convierte el UUID de empresa al int necesario en la tabla productos

DROP FUNCTION IF EXISTS fn_GetProductosByEmpresaId(UUID);

CREATE OR REPLACE FUNCTION fn_GetProductosByEmpresaId(p_empresa_id UUID)
RETURNS TABLE (
    producto_id INTEGER,
    empresa_id INTEGER,
    nombre VARCHAR(255),
    descripcion TEXT,
    fecha_creacion TIMESTAMP
) AS $$
DECLARE
    v_empresa_int_id INTEGER;
BEGIN
    -- Obtener el ID entero de la empresa basado en su UUID
    -- Ajusta esta query según tu estructura real de BD
    SELECT COALESCE(e.id::INTEGER, 0) INTO v_empresa_int_id
    FROM empresas e
    WHERE e.empresa_id::UUID = p_empresa_id
    LIMIT 1;
    
    -- Si no encuentra la empresa, retorna un conjunto vacío
    IF v_empresa_int_id = 0 THEN
        RETURN;
    END IF;

    -- Retorna todos los productos de esa empresa
    RETURN QUERY
    SELECT 
        p.producto_id,
        p.empresa_id,
        p.nombre,
        p.descripcion,
        p.fecha_creacion
    FROM productos p
    WHERE p.empresa_id = v_empresa_int_id
    ORDER BY p.fecha_creacion DESC;
END;
$$ LANGUAGE plpgsql;

-- Ejemplo de uso:
-- SELECT * FROM fn_GetProductosByEmpresaId('550e8400-e29b-41d4-a716-446655440000'::UUID);

-- ALTERNATIVA SIMPLIFICADA (si solo necesitas una relación directa):
-- CREATE OR REPLACE FUNCTION fn_GetProductosByEmpresaId(p_empresa_id UUID)
-- RETURNS TABLE (
--     producto_id INTEGER,
--     empresa_id INTEGER,
--     nombre VARCHAR(255),
--     descripcion TEXT,
--     fecha_creacion TIMESTAMP
-- ) AS $$
-- BEGIN
--     RETURN QUERY
--     SELECT 
--         p.producto_id,
--         p.empresa_id,
--         p.nombre,
--         p.descripcion,
--         p.fecha_creacion
--     FROM productos p
--     INNER JOIN empresas e ON p.empresa_id = e.id
--     WHERE e.empresa_id = p_empresa_id
--     ORDER BY p.fecha_creacion DESC;
-- END;
-- $$ LANGUAGE plpgsql;
