using MediatR;
using SIGMA.API.Appointments.DTOs;
using SIGMA.API.Common.Pagination;
using SIGMA.API.Enums;

namespace SIGMA.API.Appointments.Queries.GetAll;

// Query paginada de turnos con filtros opcionales por estado y rango de fechas
public record GetAppointmentsQuery(
    int Page = 1,
    int PageSize = 10,
    AppointmentStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null
) : IRequest<PaginatedResult<AppointmentDto>>;
