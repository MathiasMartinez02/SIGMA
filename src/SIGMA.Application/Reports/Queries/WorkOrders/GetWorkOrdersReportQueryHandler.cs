using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Reports.DTOs;

namespace SIGMA.Application.Reports.Queries.WorkOrders;

public class GetWorkOrdersReportQueryHandler : IRequestHandler<GetWorkOrdersReportQuery, WorkOrdersReportDto>
{
    private readonly IApplicationDbContext _context;

    public GetWorkOrdersReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkOrdersReportDto> Handle(GetWorkOrdersReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.WorkOrders
            .Include(w => w.Aircraft)
            .Include(w => w.Client)
            .Where(w => !w.IsDeleted
                && w.CreatedAt >= request.DateFrom
                && w.CreatedAt <= request.DateTo);

        if (request.Status.HasValue)
            query = query.Where(w => w.Status == request.Status.Value);

        if (request.Type.HasValue)
            query = query.Where(w => w.Type == request.Type.Value);

        if (request.ClientId.HasValue)
            query = query.Where(w => w.ClientId == request.ClientId.Value);

        var workOrders = await query.OrderByDescending(w => w.CreatedAt).ToListAsync(cancellationToken);

        var items = workOrders.Select(w => new WorkOrderReportItemDto
        {
            Id = w.Id,
            Number = w.Number,
            Type = w.Type.ToString(),
            Status = w.Status.ToString(),
            Priority = w.Priority.ToString(),
            AircraftRegistration = w.Aircraft.Registration,
            AircraftModel = w.Aircraft.Model,
            ClientName = w.Client.Name,
            Description = w.Description,
            EstimatedHours = w.EstimatedHours,
            ActualHours = w.ActualHours,
            StartDate = w.StartDate,
            EstimatedEndDate = w.EstimatedEndDate,
            CompletedDate = w.CompletedDate,
            CreatedAt = w.CreatedAt
        }).ToList();

        var byStatus = workOrders
            .GroupBy(w => w.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var byType = workOrders
            .GroupBy(w => w.Type.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var averageHours = workOrders.Count > 0
            ? Math.Round(workOrders.Where(w => w.ActualHours > 0).Select(w => w.ActualHours).DefaultIfEmpty(0).Average(), 2)
            : 0;

        return new WorkOrdersReportDto
        {
            WorkOrders = items,
            TotalCount = items.Count,
            AverageHours = averageHours,
            ByStatus = byStatus,
            ByType = byType
        };
    }
}
