using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Inspections.DTOs;
using SIGMA.Application.Reports.DTOs;

namespace SIGMA.Application.Reports.Queries.Inspections;

public class GetInspectionsReportQueryHandler : IRequestHandler<GetInspectionsReportQuery, InspectionsReportDto>
{
    private readonly IApplicationDbContext _context;

    public GetInspectionsReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InspectionsReportDto> Handle(GetInspectionsReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Inspections
            .Include(i => i.WorkOrder)
            .Include(i => i.Aircraft)
            .Include(i => i.Inspector)
            .Where(i => !i.IsDeleted
                && i.ScheduledDate >= request.DateFrom
                && i.ScheduledDate <= request.DateTo);

        if (!string.IsNullOrWhiteSpace(request.Type))
            query = query.Where(i => i.Type == request.Type);

        if (request.InspectorId.HasValue)
            query = query.Where(i => i.InspectorId == request.InspectorId.Value);

        var inspections = await query.OrderByDescending(i => i.ScheduledDate).ToListAsync(cancellationToken);

        var dtos = inspections.Select(i => new InspectionDto
        {
            Id = i.Id,
            WorkOrderId = i.WorkOrderId,
            WorkOrderNumber = i.WorkOrder.Number,
            Type = i.Type,
            Status = i.Status,
            AircraftId = i.AircraftId,
            AircraftRegistration = i.Aircraft.Registration,
            AircraftHours = i.AircraftHours,
            InspectorId = i.InspectorId,
            InspectorName = i.Inspector != null ? $"{i.Inspector.FirstName} {i.Inspector.LastName}" : null,
            ScheduledDate = i.ScheduledDate,
            StartedAt = i.StartedAt,
            CompletedAt = i.CompletedAt,
            ApprovedAt = i.ApprovedAt,
            OverallResult = i.OverallResult,
            Observations = i.Observations,
            RejectionReason = i.RejectionReason,
            CreatedAt = i.CreatedAt
        }).ToList();

        var byStatus = inspections
            .GroupBy(i => i.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var byType = inspections
            .GroupBy(i => i.Type)
            .ToDictionary(g => g.Key, g => g.Count());

        // Cumplimiento normativo: de las inspecciones ya completadas (con CompletedAt cargado), que porcentaje se realizo
        // en o antes de la fecha programada (ScheduledDate). Sin esto, no hay forma de medir si se cumple el cronograma de mantenimiento.
        var completed = inspections.Where(i => i.CompletedAt.HasValue).ToList();
        var onTimeCount = completed.Count(i => i.CompletedAt!.Value.Date <= i.ScheduledDate.Date);
        var compliancePercentage = completed.Count > 0
            ? Math.Round((decimal)onTimeCount / completed.Count * 100, 2)
            : (decimal?)null;

        return new InspectionsReportDto
        {
            Inspections = dtos,
            TotalCount = dtos.Count,
            ByStatus = byStatus,
            ByType = byType,
            CompletedCount = completed.Count,
            OnTimeCount = onTimeCount,
            CompliancePercentage = compliancePercentage
        };
    }
}
