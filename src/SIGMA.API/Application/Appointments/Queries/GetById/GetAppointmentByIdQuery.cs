using MediatR;
using SIGMA.API.Appointments.DTOs;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Appointments.Queries.GetById;

// Query para obtener el detalle de un turno por id
public record GetAppointmentByIdQuery(Guid Id) : IRequest<Result<AppointmentDto>>;
