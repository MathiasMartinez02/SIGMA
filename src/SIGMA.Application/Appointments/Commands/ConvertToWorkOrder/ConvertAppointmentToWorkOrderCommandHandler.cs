using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Application.WorkOrders.DTOs;
using SIGMA.Domain.Entities;
using SIGMA.Domain.Enums;
using SIGMA.Domain.Exceptions;

namespace SIGMA.Application.Appointments.Commands.ConvertToWorkOrder;

// Handler que crea la OT a partir de un turno confirmado, reusando la logica de numeracion de CreateWorkOrderCommandHandler
public class ConvertAppointmentToWorkOrderCommandHandler
    : IRequestHandler<ConvertAppointmentToWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IWorkOrderNumberGenerator _numberGenerator;

    public ConvertAppointmentToWorkOrderCommandHandler(
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

    public async Task<Result<WorkOrderDto>> Handle(ConvertAppointmentToWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

        if (appointment is null)
            return Result<WorkOrderDto>.Failure("Turno no encontrado.");

        if (appointment.AircraftId is null)
            return Result<WorkOrderDto>.Failure("Debe registrar la aeronave antes de convertir el turno en una orden de trabajo.");

        var aircraft = await _context.Aircraft
            .FirstOrDefaultAsync(a => a.Id == appointment.AircraftId.Value && !a.IsDeleted, cancellationToken);

        if (aircraft is null)
            return Result<WorkOrderDto>.Failure("La aeronave no fue encontrada.");

        var number = await _numberGenerator.GenerateAsync(cancellationToken);
        var userId = _currentUser.UserId!.Value;
        var intakeDate = DateTime.UtcNow;
        var description = "Generada desde turno confirmado."
            + (string.IsNullOrWhiteSpace(appointment.Notes) ? "" : $" Notas: {appointment.Notes}");

        WorkOrder workOrder;
        try
        {
            workOrder = WorkOrder.Create(
                number, appointment.RequestedType, WorkOrderPriority.Media,
                aircraft.Id, aircraft.ClientId, description,
                estimatedHours: 1, intakeDate, intakeDate.AddDays(7),
                aircraft.TotalFlightHours, userId);

            appointment.MarkConverted(workOrder.Id);
        }
        catch (DomainException ex)
        {
            return Result<WorkOrderDto>.Failure(ex.Message);
        }

        var timeline = WorkOrderTimeline.Create(
            workOrder.Id, "Creación", $"Orden de trabajo {number} creada a partir del turno confirmado",
            userId, _currentUser.FullName ?? "Sistema", _currentUser.Role ?? "sistema");

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
