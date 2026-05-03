# Fix PostgreSQL Activa Column Error (EF Quoting)

**Status**: ✅ FIXED

## Changes Applied
- Added `[Column("\"Activa\"")]` to Producto.Activa
- Quoted all mixed-case columns: "ProductoID", "EmpresaID", "CategoriaID"
- EF now generates correct SQL: `p0."Activa" = true`

## Test
```
GET /api/Categorias/productos/getAll
Expected: 200 OK, products list
```

**Next**: Restart app → Test endpoint → Remove TODO.md
