using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.WorkOrders.DTOs;
using SIGMA.API.Enums;

namespace SIGMA.API.Aircraft.Queries.GetMaintenanceHistory;

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

        IList<WorkOrderDto> dtos = await _context.WorkOrders
            .Where(w => w.AircraftId == request.AircraftId && !w.IsDeleted)
            .OrderByDescending(w => w.CreatedAt)
            .Select(WorkOrderProjections.ToDto)
            .ToListAsync(cancellationToken);

        return Result<IList<WorkOrderDto>>.Success(dtos);
    }
}
