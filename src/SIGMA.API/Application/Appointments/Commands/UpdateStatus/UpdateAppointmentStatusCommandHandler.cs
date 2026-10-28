using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.API.Common.Interfaces;
using SIGMA.API.Common.Models;
using SIGMA.API.Enums;
using SIGMA.API.Exceptions;

namespace SIGMA.API.Appointments.Commands.UpdateStatus;

// Handler que aplica Confirm() o Cancel() sobre el turno segun el nuevo estado solicitado
public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateAppointmentStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        if (request.NewStatus != AppointmentStatus.Confirmado && request.NewStatus != AppointmentStatus.Cancelado)
            return Result.Failure("Desde este endpoint solo se puede confirmar o cancelar el turno.");

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (appointment is null)
            return Result.Failure("Turno no encontrado.");

        try
        {
            if (request.NewStatus == AppointmentStatus.Confirmado)
                appointment.Confirm();
            else
                appointment.Cancel();

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
