using MediatR;
using SIGMA.Application.Common.Models;
using SIGMA.Application.Inspections.DTOs;

namespace SIGMA.Application.Inspections.Queries.GetById;

public record GetInspectionByIdQuery(Guid Id) : IRequest<Result<InspectionDetailDto>>;
