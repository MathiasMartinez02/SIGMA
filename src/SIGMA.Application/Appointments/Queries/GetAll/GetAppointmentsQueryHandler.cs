using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGMA.Application.Appointments.DTOs;
using SIGMA.Application.Common.Interfaces;
using SIGMA.Application.Common.Pagination;

namespace SIGMA.Application.Appointments.Queries.GetAll;

// Handler que filtra y pagina los turnos, ordenados por fecha programada ascendente
public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, PaginatedResult<AppointmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAppointmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<AppointmentDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Appointments
            .Include(a => a.Client)
            .Include(a => a.Aircraft)
            .Where(a => !a.IsDeleted)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(a => a.Status == request.Status.Value);

        if (request.DateFrom.HasValue)
            query = query.Where(a => a.ScheduledDate >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(a => a.ScheduledDate <= request.DateTo.Value);

        query = query.OrderBy(a => a.ScheduledDate);

        var mapped = query.Select(a => new AppointmentDto
        {
            Id = a.Id,
            ClientId = a.ClientId,
            ClientName = a.Client.Name,
            AircraftId = a.AircraftId,
            AircraftRegistration = a.Aircraft != null ? a.Aircraft.Registration : a.AircraftRegistrationHint,
            RequestedType = a.RequestedType,
            ScheduledDate = a.ScheduledDate,
            Status = a.Status,
            Notes = a.Notes,
            ConvertedWorkOrderId = a.ConvertedWorkOrderId,
            CreatedAt = a.CreatedAt
        });

        return await mapped.ToPaginatedResultAsync(request.Page, request.PageSize, cancellationToken);
    }
}
