using MediatR;
using SIGMA.API.Common.Models;
using SIGMA.API.Inspections.DTOs;

namespace SIGMA.API.Inspections.Queries.GetById;

public record GetInspectionByIdQuery(Guid Id) : IRequest<Result<InspectionDetailDto>>;
