using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Persistence;

namespace SIGMA.API.Services;

public class WorkOrderNumberGenerator : IWorkOrderNumberGenerator
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    private static readonly Dictionary<int, int> LastIssuedByYear = [];

    private readonly ApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTime;

    public WorkOrderNumberGenerator(ApplicationDbContext context, IDateTimeProvider dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    // Proximo numero OT-AAAA-NNNN; reserva el emitido para que dos altas simultaneas no repitan
    public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        await Lock.WaitAsync(cancellationToken);
        try
        {
            var year = _dateTime.CurrentYear;
            var prefix = $"OT-{year}-";

            var existing = await _context.WorkOrders
                .IgnoreQueryFilters()
                .Where(w => w.Number.StartsWith(prefix))
                .Select(w => w.Number)
                .ToListAsync(cancellationToken);

            var maxInDatabase = existing
                .Select(n => int.TryParse(n[prefix.Length..], out var value) ? value : 0)
                .DefaultIfEmpty(0)
                .Max();

            var next = Math.Max(maxInDatabase, LastIssuedByYear.GetValueOrDefault(year)) + 1;
            LastIssuedByYear[year] = next;

            return $"{prefix}{next:D4}";
        }
        finally
        {
            Lock.Release();
        }
    }
}
