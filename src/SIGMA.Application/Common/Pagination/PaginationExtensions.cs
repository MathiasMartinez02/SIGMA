using Microsoft.EntityFrameworkCore;

namespace SIGMA.Application.Common.Pagination;

public static class PaginationExtensions
{
    public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PaginatedResult<T>.Create(items, total, page, pageSize);
    }
}
