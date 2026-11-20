using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Exceptions;

namespace SIGMA.API.Appointments.Commands.Update;

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateAppointmentCommandHandler(IApplicationDbContext context) => _context = context;

    // Reprograma el turno o le asocia una aeronave del mismo cliente
    public async Task<Result> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (appointment is null)
            return Result.Failure("Turno no encontrado.");

        if (request.AircraftId.HasValue)
        {
            var aircraft = await _context.Aircraft
                .FirstOrDefaultAsync(a => a.Id == request.AircraftId.Value && !a.IsDeleted, cancellationToken);

            if (aircraft is null)
                return Result.Failure("La aeronave no fue encontrada.");
            if (aircraft.ClientId != appointment.ClientId)
                return Result.Failure("La aeronave seleccionada no pertenece al cliente del turno.");
        }

        try
        {
            appointment.Update(request.AircraftId, request.AircraftRegistrationHint, request.RequestedType,
                request.ScheduledDate, request.Notes);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
