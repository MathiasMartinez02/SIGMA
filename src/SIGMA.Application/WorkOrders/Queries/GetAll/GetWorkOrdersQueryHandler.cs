using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Pagination;
using SIGMA.Application.WorkOrders.DTOs;

namespace SIGMA.Application.WorkOrders.Queries.GetAll;

public class GetWorkOrdersQueryHandler : IRequestHandler<GetWorkOrdersQuery, PaginatedResult<WorkOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetWorkOrdersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<WorkOrderDto>> Handle(GetWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.WorkOrders
            .Include(w => w.Aircraft)
            .Include(w => w.Client)
            .Include(w => w.Tasks)
            .Where(w => !w.IsDeleted)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(w => w.Status == request.Status.Value);

        if (request.Priority.HasValue)
            query = query.Where(w => w.Priority == request.Priority.Value);

        if (request.Type.HasValue)
            query = query.Where(w => w.Type == request.Type.Value);

        if (request.AircraftId.HasValue)
            query = query.Where(w => w.AircraftId == request.AircraftId.Value);

        if (request.ClientId.HasValue)
            query = query.Where(w => w.ClientId == request.ClientId.Value);

        if (request.MechanicId.HasValue)
            query = query.Where(w => w.AssignedMechanics.Any(am => am.UserId == request.MechanicId.Value));

        if (request.DateFrom.HasValue)
            query = query.Where(w => w.CreatedAt >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(w => w.CreatedAt <= request.DateTo.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(w =>
                w.Number.ToLower().Contains(search) ||
                w.Description.ToLower().Contains(search) ||
                w.Aircraft.Registration.ToLower().Contains(search) ||
                w.Client.Name.ToLower().Contains(search));
        }

        query = query.OrderByDescending(w => w.CreatedAt);

        var mapped = query.Select(w => new WorkOrderDto
        {
            Id = w.Id,
            Number = w.Number,
            Type = w.Type,
            Status = w.Status,
            Priority = w.Priority,
            AircraftId = w.AircraftId,
            AircraftRegistration = w.Aircraft.Registration,
            AircraftModel = w.Aircraft.Model,
            ClientId = w.ClientId,
            ClientName = w.Client.Name,
            Description = w.Description,
            EstimatedHours = w.EstimatedHours,
            ActualHours = w.ActualHours,
            StartDate = w.StartDate,
            EstimatedEndDate = w.EstimatedEndDate,
            CompletedDate = w.CompletedDate,
            AircraftHoursAtStart = w.AircraftHoursAtStart,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt,
            TaskCount = w.Tasks.Count,
            CompletedTaskCount = w.Tasks.Count(t => t.Status == Domain.Enums.WorkOrderTaskStatus.Completada)
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
