using MediatR;
using SIGMA.API.Common.Models;

namespace SIGMA.API.Inspections.Commands.Start;

public record StartInspectionCommand(Guid InspectionId) : IRequest<Result>;
