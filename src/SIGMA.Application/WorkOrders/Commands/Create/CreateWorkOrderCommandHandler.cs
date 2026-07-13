using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;
using SIGMA.Domain.Entities;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Application.WorkOrders.Commands.Create;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IWorkOrderNumberGenerator _numberGenerator;

    public CreateWorkOrderCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUser,
        IWorkOrderNumberGenerator numberGenerator)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _numberGenerator = numberGenerator;
    }

    public async Task<Result<WorkOrderDto>> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var aircraft = await _context.Aircraft
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == request.AircraftId && !a.IsDeleted, cancellationToken);

        if (aircraft is null)
            return Result<WorkOrderDto>.Failure("La aeronave no fue encontrada.");

        if (aircraft.Status == AircraftStatus.FueraDeServicio)
            return Result<WorkOrderDto>.Failure("No se puede crear una OT para una aeronave fuera de servicio.");

        var number = await _numberGenerator.GenerateAsync(cancellationToken);
        var userId = _currentUser.UserId!.Value;

        var workOrder = WorkOrder.Create(
            number, request.Type, request.Priority,
            aircraft.Id, aircraft.ClientId,
            request.Description, request.EstimatedHours,
            request.IntakeDate, request.EstimatedEndDate, aircraft.TotalFlightHours, userId);

        var timeline = WorkOrderTimeline.Create(
            workOrder.Id, "Creación", $"Orden de trabajo {number} creada",
            userId, _currentUser.FullName ?? "Sistema",
            _currentUser.Role ?? "sistema");

        _context.WorkOrders.Add(workOrder);
        _context.WorkOrderTimelines.Add(timeline);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.WorkOrders
            .Include(w => w.Aircraft)
            .Include(w => w.Client)
            .Include(w => w.Tasks)
            .FirstAsync(w => w.Id == workOrder.Id, cancellationToken);

        return Result<WorkOrderDto>.Success(_mapper.Map<WorkOrderDto>(created));
    }
}
