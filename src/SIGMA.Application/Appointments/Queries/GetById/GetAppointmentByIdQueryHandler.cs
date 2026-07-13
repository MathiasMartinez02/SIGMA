using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Appointments.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Appointments.Queries.GetById;

// Handler que busca un turno por id y resuelve la matricula real o la de referencia
public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAppointmentByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AppointmentDto>> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Client)
            .Include(a => a.Aircraft)
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (appointment is null)
            return Result<AppointmentDto>.Failure("Turno no encontrado.");

        return Result<AppointmentDto>.Success(new AppointmentDto
        {
            Id = appointment.Id,
            ClientId = appointment.ClientId,
            ClientName = appointment.Client.Name,
            AircraftId = appointment.AircraftId,
            AircraftRegistration = appointment.Aircraft?.Registration ?? appointment.AircraftRegistrationHint,
            RequestedType = appointment.RequestedType,
            ScheduledDate = appointment.ScheduledDate,
            Status = appointment.Status,
            Notes = appointment.Notes,
            ConvertedWorkOrderId = appointment.ConvertedWorkOrderId,
            CreatedAt = appointment.CreatedAt
        });
    }
}
