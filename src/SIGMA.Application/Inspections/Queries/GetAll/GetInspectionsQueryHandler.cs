using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.Inspections.DTOs;

namespace SIGMA.Application.Inspections.Queries.GetAll;

public class GetInspectionsQueryHandler : IRequestHandler<GetInspectionsQuery, PaginatedResult<InspectionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInspectionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<InspectionDto>> Handle(GetInspectionsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Inspections
            .Include(i => i.WorkOrder)
            .Include(i => i.Aircraft)
            .Include(i => i.Inspector)
            .Where(i => !i.IsDeleted)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(i => i.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.Type))
            query = query.Where(i => i.Type == request.Type);

        if (request.InspectorId.HasValue)
            query = query.Where(i => i.InspectorId == request.InspectorId.Value);

        if (request.AircraftId.HasValue)
            query = query.Where(i => i.AircraftId == request.AircraftId.Value);

        if (request.DateFrom.HasValue)
            query = query.Where(i => i.ScheduledDate >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(i => i.ScheduledDate <= request.DateTo.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(i =>
                i.Type.ToLower().Contains(search) ||
                i.Aircraft.Registration.ToLower().Contains(search) ||
                i.WorkOrder.Number.ToLower().Contains(search));
        }

        query = query.OrderByDescending(i => i.ScheduledDate);

        var mapped = query.Select(i => new InspectionDto
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
            InspectorName = i.Inspector != null ? i.Inspector.FirstName + " " + i.Inspector.LastName : null,
            ScheduledDate = i.ScheduledDate,
            StartedAt = i.StartedAt,
            CompletedAt = i.CompletedAt,
            ApprovedAt = i.ApprovedAt,
            OverallResult = i.OverallResult,
            Observations = i.Observations,
            RejectionReason = i.RejectionReason,
            CreatedAt = i.CreatedAt
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
