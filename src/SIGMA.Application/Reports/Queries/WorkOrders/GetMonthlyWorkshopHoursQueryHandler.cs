using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Reports.DTOs;

namespace SIGMA.Application.Reports.Queries.WorkOrders;

public class GetMonthlyWorkshopHoursQueryHandler : IRequestHandler<GetMonthlyWorkshopHoursQuery, MonthlyWorkshopHoursReportDto>
{
    private readonly IApplicationDbContext _context;

    public GetMonthlyWorkshopHoursQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    // Agrupa las horas reales (ActualHours) de las OT por mes calendario, usando CompletedDate como fecha de referencia
    // (o StartDate/CreatedAt como respaldo para OT sin fecha de finalizacion), para los ultimos N meses incluyendo el actual
    public async Task<MonthlyWorkshopHoursReportDto> Handle(GetMonthlyWorkshopHoursQuery request, CancellationToken cancellationToken)
    {
        var months = request.Months <= 0 ? 7 : request.Months;
        var now = DateTime.UtcNow;
        var rangeStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-(months - 1));

        var workOrders = await _context.WorkOrders
            .Where(w => !w.IsDeleted && w.ActualHours > 0)
            .Select(w => new { w.ActualHours, w.CompletedDate, w.StartDate, w.CreatedAt })
            .ToListAsync(cancellationToken);

        var buckets = new Dictionary<(int Year, int Month), decimal>();
        foreach (var w in workOrders)
        {
            var referenceDate = w.CompletedDate ?? w.StartDate ?? w.CreatedAt;
            if (referenceDate < rangeStart) continue;

            var key = (referenceDate.Year, referenceDate.Month);
            buckets[key] = buckets.GetValueOrDefault(key) + w.ActualHours;
        }

        var items = new List<MonthlyWorkshopHoursItemDto>();
        for (var i = 0; i < months; i++)
        {
            var cursor = rangeStart.AddMonths(i);
            var key = (cursor.Year, cursor.Month);
            items.Add(new MonthlyWorkshopHoursItemDto
            {
                Year = cursor.Year,
                Month = cursor.Month,
                Label = cursor.ToString("MMM", CultureInfo.GetCultureInfo("es-AR")),
                TotalHours = Math.Round(buckets.GetValueOrDefault(key), 2)
            });
        }

        return new MonthlyWorkshopHoursReportDto { Items = items };
    }
}
