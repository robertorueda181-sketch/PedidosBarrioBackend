-- ============================================================
-- FUNCTION: sp_GetNegocioByCodigoEmpresa
-- Devuelve datos de la empresa asociada a un negocio por código
-- También devuelve productos de esa empresa
-- ============================================================

-- 1. ELIMINAR FUNCIÓN ANTERIOR
DROP FUNCTION IF EXISTS public.sp_getnegociobyid(character varying);

-- 2. CREAR FUNCIÓN PARA OBTENER DATOS DE LA EMPRESA POR CÓDIGO
CREATE OR REPLACE FUNCTION public.sp_getnegociobycodigoempresa(p_codigo_empresa character varying)
RETURNS TABLE(
    "Nombre" character varying,        
    "Descripcion" text,
    "Email" character varying,
    "Telefono" character varying,
    "Direccion" character varying,
    "Referencia" character varying
) 
LANGUAGE 'plpgsql'
STABLE 
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT 
        e."Nombre",        
        e."Descripcion",
        e."Email",
        e."Telefono",
        e."Direccion",
        e."Referencia"
    FROM public."Negocios" n
    INNER JOIN public."Empresas" e ON n."EmpresaID" = e."EmpresaID"
    WHERE e."Codigo" = p_codigo_empresa;
END;
$BODY$;

ALTER FUNCTION public.sp_getnegociobycodigoempresa(character varying) OWNER TO postgres;

-- ============================================================
-- FUNCTION: sp_GetProductosByCodigoEmpresa
-- Devuelve TODOS los productos de una empresa por código
-- ============================================================

DROP FUNCTION IF EXISTS public.sp_getproductosbycodigoempresa(character varying);

CREATE OR REPLACE FUNCTION public.sp_getproductosbycodigoempresa(p_codigo_empresa character varying)
RETURNS TABLE(
    "ProductoID" integer,
    "EmpresaID" uuid,
    "Nombre" character varying,
    "Descripcion" text,
    "Precio" numeric,
    "Stock" integer,
    "FechaRegistro" timestamp with time zone,
    "URLImagen" character varying
)
LANGUAGE 'plpgsql'
STABLE 
ROWS 1000
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT 
        p."ProductoID",
        p."EmpresaID",
        p."Nombre",
        p."Descripcion",
        p."Precio",
        p."Stock",
        p."FechaRegistro",
        im."URLImagen"
    FROM public."Productos" p
    INNER JOIN public."Empresas" e ON p."EmpresaID" = e."EmpresaID"
    LEFT JOIN public."Imagenes" im 
        ON p."ProductoID" = im."ExternalId" 
        AND im."Type" = 'prod'
        AND im."Activa" = TRUE
    WHERE e."Codigo" = p_codigo_empresa
    AND p."Activa" = TRUE
    ORDER BY p."FechaRegistro" DESC;
END;
$BODY$;

ALTER FUNCTION public.sp_getproductosbycodigoempresa(character varying) OWNER TO postgres;
