using Microsoft.EntityFrameworkCore;
using PedidosBarrio.Domain.Entities;
using PedidosBarrio.Domain.Repositories;
using PedidosBarrio.Infrastructure.Data.Contexts;

namespace PedidosBarrio.Infrastructure.Data.Repositories;

public class PageViewRepository : IPageViewRepository
{
    private readonly PedidosBarrioDbContext _context;

    public PageViewRepository(PedidosBarrioDbContext context)
    {
        _context = context;
    }

    public async Task<PageView> AddAsync(PageView pageView)
    {
        _context.PageViews.Add(pageView);
        await _context.SaveChangesAsync();
        return pageView;
    }

    public async Task<int> AddBulkAsync(IEnumerable<PageView> pageViews)
    {
        var pageViewsList = pageViews.ToList();
        if (pageViewsList.Count == 0)
            return 0;

        _context.PageViews.AddRange(pageViewsList);
        return await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PageView>> GetByEmpresaAndDateRangeAsync(
        Guid empresaId, 
        DateTime startDate, 
        DateTime endDate)
    {
        return await _context.PageViews
            .AsNoTracking()
            .Where(pv => pv.EmpresaID == empresaId 
                && pv.Fecha >= startDate 
                && pv.Fecha <= endDate)
            .OrderByDescending(pv => pv.Fecha)
            .ToListAsync();
    }

    public async Task<int> GetCountByEmpresaAsync(Guid empresaId)
    {
        return await _context.PageViews
            .AsNoTracking()
            .CountAsync(pv => pv.EmpresaID == empresaId);
    }

    public async Task<IEnumerable<(string Url, int Count)>> GetTopUrlsByEmpresaAsync(
        Guid empresaId, 
        int topCount = 10)
    {
        return await _context.PageViews
            .AsNoTracking()
            .Where(pv => pv.EmpresaID == empresaId)
            .GroupBy(pv => pv.Url)
            .Select(g => new { Url = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(topCount)
            .AsAsyncEnumerable()
            .Select(x => (x.Url, x.Count))
            .ToListAsync();
    }

    public async Task<int> MarkAsProcessedAsync(IEnumerable<int> pageViewIds)
    {
        var ids = pageViewIds.ToList();
        if (ids.Count == 0)
            return 0;

        return await _context.PageViews
            .Where(pv => ids.Contains(pv.PageViewID))
            .ExecuteUpdateAsync(s => s
                .SetProperty(pv => pv.Processed, true)
                .SetProperty(pv => pv.ProcessedAt, DateTime.UtcNow)
            );
    }
}
