using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Aircraft.Queries.GetMaintenanceHistory;

public class GetMaintenanceHistoryQueryHandler : IRequestHandler<GetMaintenanceHistoryQuery, Result<IList<WorkOrderDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMaintenanceHistoryQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IList<WorkOrderDto>>> Handle(GetMaintenanceHistoryQuery request, CancellationToken cancellationToken)
    {
        var aircraftExists = await _context.Aircraft
            .AnyAsync(a => a.Id == request.AircraftId && !a.IsDeleted, cancellationToken);

        if (!aircraftExists)
            return Result<IList<WorkOrderDto>>.Failure("La aeronave no fue encontrada.");

        var workOrders = await _context.WorkOrders
            .Include(w => w.Aircraft)
            .Include(w => w.Client)
            .Include(w => w.Tasks)
            .Where(w => w.AircraftId == request.AircraftId && !w.IsDeleted)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = workOrders.Select(w => new WorkOrderDto
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
            CompletedTaskCount = w.Tasks.Count(t => t.Status == WorkOrderTaskStatus.Completada)
        }).ToList();

        return Result<IList<WorkOrderDto>>.Success(dtos);
    }
}
