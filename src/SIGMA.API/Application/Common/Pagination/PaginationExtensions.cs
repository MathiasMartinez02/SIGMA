using Microsoft.EntityFrameworkCore;

namespace SIGMA.API.Common.Pagination;

public static class PaginationExtensions
{
    public const int MaxPageSize = 100;

    // Pagina la consulta normalizando pagina (minimo 1) y tamaño (entre 1 y 100)
    public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PaginatedResult<T>.Create(items, total, page, pageSize);
    }
}
