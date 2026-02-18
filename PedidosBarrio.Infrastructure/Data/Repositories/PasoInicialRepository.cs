using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;
using PedidosBarrio.Infrastructure.Data.Repositories.Base;

namespace PedidosBarrio.Infrastructure.Data.Repositories;

public class PasoInicialRepository : EfCoreRepository<PasoInicial>, IPasoInicialRepository
{
    private readonly PedidosBarrioDbContext _context;

    public PasoInicialRepository(PedidosBarrioDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PasoInicial>> GetPasosPorEmpresaAsync(Guid empresaId)
    {
        return await _context.PasosIniciales
            .AsNoTracking()
            .Where(p => p.EmpresaID == empresaId && p.Activo)
            .OrderBy(p => p.Orden)
            .ToListAsync();
    }

    public async Task<PasoInicial> GetByIdAsync(int pasoId)
    {
        return await _context.PasosIniciales
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PasoID == pasoId && p.Activo);
    }

    public async Task<int> AddAsync(PasoInicial paso)
    {
        paso.FechaCreacion = DateTime.UtcNow;
        _context.PasosIniciales.Add(paso);
        await _context.SaveChangesAsync();
        return paso.PasoID;
    }

    public async Task UpdateAsync(PasoInicial paso)
    {
        var existingPaso = await _context.PasosIniciales
            .FirstOrDefaultAsync(p => p.PasoID == paso.PasoID);

        if (existingPaso != null)
        {
            existingPaso.Completado = paso.Completado;
            if (paso.Completado && !existingPaso.FechaCompletado.HasValue)
            {
                existingPaso.FechaCompletado = DateTime.UtcNow;
            }

            // Normalizar FechaCreacion a UTC si es necesario
            if (existingPaso.FechaCreacion.Kind != DateTimeKind.Utc)
            {
                existingPaso.FechaCreacion = DateTime.SpecifyKind(existingPaso.FechaCreacion, DateTimeKind.Utc);
            }

            _context.PasosIniciales.Update(existingPaso);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int pasoId)
    {
        var paso = await _context.PasosIniciales
            .FirstOrDefaultAsync(p => p.PasoID == pasoId);

        if (paso != null)
        {
            paso.Activo = false;
            _context.PasosIniciales.Update(paso);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> CompletarPasoAsync(int pasoId)
    {
        var paso = await _context.PasosIniciales
            .FirstOrDefaultAsync(p => p.PasoID == pasoId && p.Activo);

        if (paso != null)
        {
            paso.Completado = true;
            paso.FechaCompletado = DateTime.UtcNow;

            // Normalizar FechaCreacion a UTC si es necesario
            if (paso.FechaCreacion.Kind != DateTimeKind.Utc)
            {
                paso.FechaCreacion = DateTime.SpecifyKind(paso.FechaCreacion, DateTimeKind.Utc);
            }

            _context.PasosIniciales.Update(paso);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task CrearPasosInicialesDefaultAsync(Guid empresaId)
    {
        // Verificar si ya existen pasos iniciales para esta empresa
        var existentes = await _context.PasosIniciales
            .Where(p => p.EmpresaID == empresaId)
            .CountAsync();

        if (existentes > 0)
            return;

        // Crear pasos iniciales por defecto
        var pasosDefault = new[]
        {
            new PasoInicial
            {
                EmpresaID = empresaId,
                Codigo = "COMPLETAR_PERFIL",
                Titulo = "Completa tu perfil",
                Descripcion = "Agrega una descripción",
                Icono = "user-circle",
                Ruta = "/empresa/perfil",
                Obligatorio = true,
                Orden = 1,
                Completado = false,
                Activo = true
            },
            new PasoInicial
            {
                EmpresaID = empresaId,
                Codigo = "CAMBIAR_LOGO",
                Titulo = "Cambia el logo de tu empresa",
                Descripcion = "Sube el logo que representará a tu negocio",
                Icono = "image",
                Ruta = "/empresa/perfil",
                Obligatorio = true,
                Orden = 2,
                Completado = false,
                Activo = true
            },
            new PasoInicial
            {
                EmpresaID = empresaId,
                Codigo = "COMPLETAR_DIRECCION",
                Titulo = "Completa tu dirección",
                Descripcion = "Ingresa la dirección completa de tu negocio",
                Icono = "map-pin",
                Ruta = "/empresa/direccion",
                Obligatorio = true,
                Orden = 3,
                Completado = false,
                Activo = true
            },
            new PasoInicial
            {
                EmpresaID = empresaId,
                Codigo = "CREAR_PRODUCTO",
                Titulo = "Crea tu primer producto",
                Descripcion = "Publica tu primer producto o servicio",
                Icono = "plus-circle",
                Ruta = "/productos/crear",
                Obligatorio = true,
                Orden = 4,
                Completado = false,
                Activo = true
            }
        };

                _context.PasosIniciales.AddRange(pasosDefault);
                await _context.SaveChangesAsync();
            }

            public async Task<bool> TienePasosPendientesAsync(Guid empresaId)
            {
                return await _context.PasosIniciales
                    .AnyAsync(p => p.EmpresaID == empresaId 
                        && p.Activo 
                        && !p.Completado);
            }

            public async Task<PasoInicial> GetPasoPorCodigoAsync(Guid empresaId, string codigo)
            {
                return await _context.PasosIniciales
                    .FirstOrDefaultAsync(p => p.EmpresaID == empresaId 
                        && p.Codigo == codigo 
                        && p.Activo);
            }
        }
