using MediatR;
using SIGMA.Application.Aircraft.DTOs;
using SIGMA.Application.Common.Models;

namespace SIGMA.Application.Aircraft.Queries.GetById;

public record GetAircraftByIdQuery(Guid Id) : IRequest<Result<AircraftDetailDto>>;
