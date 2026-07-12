using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Infrastructure.Persistence;

namespace SIGMA.Infrastructure.Services;

public class WorkOrderNumberGenerator : IWorkOrderNumberGenerator
{
    private readonly ApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTime;
    private static readonly SemaphoreSlim _lock = new(1, 1);

    public WorkOrderNumberGenerator(ApplicationDbContext context, IDateTimeProvider dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var year = _dateTime.CurrentYear;
            var prefix = $"OT-{year}-";

            var lastNumber = await _context.WorkOrders
                .IgnoreQueryFilters()
                .Where(w => w.Number.StartsWith(prefix))
                .OrderByDescending(w => w.Number)
                .Select(w => w.Number)
                .FirstOrDefaultAsync(cancellationToken);

            int sequence = 1;
            if (lastNumber is not null)
            {
                var parts = lastNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out var last))
                    sequence = last + 1;
            }

            return $"{prefix}{sequence:D4}";
        }
        finally
        {
            _lock.Release();
        }
    }
}
