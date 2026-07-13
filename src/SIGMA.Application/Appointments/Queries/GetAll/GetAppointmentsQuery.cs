using MediatR;
using SIGMA.Application.Appointments.DTOs;
using SIGMA.Application.Common.Pagination;
using SIGMA.Domain.Enums;

namespace SIGMA.Application.Appointments.Queries.GetAll;

// Query paginada de turnos con filtros opcionales por estado y rango de fechas
public record GetAppointmentsQuery(
    int Page = 1,
    int PageSize = 10,
    AppointmentStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null
) : IRequest<PaginatedResult<AppointmentDto>>;
