using MediatR;
using SIGMA.API.Aircraft.DTOs;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Aircraft.Queries.GetById;

public record GetAircraftByIdQuery(Guid Id) : IRequest<Result<AircraftDetailDto>>;
