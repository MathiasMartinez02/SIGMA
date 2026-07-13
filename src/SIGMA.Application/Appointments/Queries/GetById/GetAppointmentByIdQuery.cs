using MediatR;
using SIGMA.Application.Appointments.DTOs;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Appointments.Queries.GetById;

// Query para obtener el detalle de un turno por id
public record GetAppointmentByIdQuery(Guid Id) : IRequest<Result<AppointmentDto>>;
