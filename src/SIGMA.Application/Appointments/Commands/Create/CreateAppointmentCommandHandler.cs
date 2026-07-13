using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Appointments.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;
using SIGMA.Domain.Entities;

namespace SIGMA.Application.Appointments.Commands.Create;

// Handler que valida existencia del cliente (y aeronave si se indico) y crea el turno en estado Solicitado
public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Result<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateAppointmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AppointmentDto>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _context.Clients
            .AnyAsync(c => c.Id == request.ClientId && !c.IsDeleted, cancellationToken);

        if (!clientExists)
            return Result<AppointmentDto>.Failure("El cliente no fue encontrado.");

        // MODIFICADO: se agrega la validacion de que la aeronave pertenezca al cliente del turno (antes solo se
        // verificaba que la aeronave existiera, permitiendo crear un turno con un cliente y una aeronave de otro dueño)
        // ANTERIOR: solo validaba a.Id == request.AircraftId.Value && !a.IsDeleted, sin comparar el ClientId
        if (request.AircraftId.HasValue)
        {
            var aircraft = await _context.Aircraft
                .FirstOrDefaultAsync(a => a.Id == request.AircraftId.Value && !a.IsDeleted, cancellationToken);

            if (aircraft is null)
                return Result<AppointmentDto>.Failure("La aeronave no fue encontrada.");

            if (aircraft.ClientId != request.ClientId)
                return Result<AppointmentDto>.Failure("La aeronave seleccionada no pertenece al cliente indicado.");
        }

        var appointment = Appointment.Create(
            request.ClientId, request.AircraftId, request.AircraftRegistrationHint,
            request.RequestedType, request.ScheduledDate, request.Notes);

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.Appointments
            .Include(a => a.Client)
            .Include(a => a.Aircraft)
            .FirstAsync(a => a.Id == appointment.Id, cancellationToken);

        return Result<AppointmentDto>.Success(new AppointmentDto
        {
            Id = created.Id,
            ClientId = created.ClientId,
            ClientName = created.Client.Name,
            AircraftId = created.AircraftId,
            AircraftRegistration = created.Aircraft?.Registration ?? created.AircraftRegistrationHint,
            RequestedType = created.RequestedType,
            ScheduledDate = created.ScheduledDate,
            Status = created.Status,
            Notes = created.Notes,
            ConvertedWorkOrderId = created.ConvertedWorkOrderId,
            CreatedAt = created.CreatedAt
        });
    }
}
